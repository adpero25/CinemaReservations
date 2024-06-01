using Cinema_Reservations.DAL;
using Cinema_Reservations.Exceptions;
using Cinema_Reservations.Models;
using System.Buffers.Text;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.WebSockets;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace Cinema_Reservations.WS
{
	[ServiceContract]
	public interface ICinemaService
	{
		[OperationContract]
		void MakeReservation(DateTime dateTime, int userId, int playingId, string reservedSeats);
		[OperationContract]
		void CancelReservation(int reservationId);
		[OperationContract]
		string GetReservations(int playingId);
		[OperationContract]
		List<Playing> GetRepertoire();
		[OperationContract]
		string GetMovieImage(int movieId);
		[OperationContract]
		public int CanMakeReservation(int playingId, string seats);
		[OperationContract]
		public int CanEditReservation(int reservationId, int playingId, string seats);
		[OperationContract]
		public Playing GetPlayingDetails(int playingId);
		[OperationContract]
		public List<Reservation> GetAllUserReservation(int userId);
		[OperationContract]
		public List<Reservation> GetUserReservation(int userId, int playingId);
		[OperationContract]
		public Reservation GetReservation(int userId, int reservationId);
		[OperationContract]
		public void EditReservation(int userId, int playingId, int reservationId, string seats);
	}

	public class CinemaService : ICinemaService
	{
		private readonly CinemaContext context;

		public CinemaService(CinemaContext _context)
		{
			this.context = _context;
		}

		/// <summary>
		///		Creates a reservation for a playing
		/// </summary>
		/// <param name="dateTime">Date of playing</param>
		/// <param name="userId">User id</param>
		/// <param name="playingId">Playing Id</param>
		/// <param name="reservedSeats">List of reserved seats, separated by comma</param>
		public void MakeReservation(DateTime dateTime, int userId, int playingId, string reservedSeats)
		{
			User user;
			Playing playing;

			try
			{
				user = context.Users.First(u => u.Id == userId);
				playing = context.Playings.First(p => p.Id == playingId);
			}
			catch { throw new MakeReservationException(); }

			if (string.IsNullOrEmpty(reservedSeats) || Regex.IsMatch(reservedSeats, "[a-zA-Z]+"))
				throw new MakeReservationException();

			if (playing.CanMakeReservation(reservedSeats) != "0")
				throw new MakeReservationException("Wybrane miejsca są już zajęte");

			var seatCount = reservedSeats.Split(",").Count();

			var reservation = new Reservation()
			{
				UserId = userId,
				PlayingId = playingId,
				SeatCount = seatCount,
				Seats = reservedSeats,
				Date = dateTime,
				ReservationCost = seatCount * playing.TicketCost
			};

			context.Reservations.Add(reservation);
			context.SaveChanges();
		}


		/// <summary>
		///		Usuwa rezerwację o podanym Id
		/// </summary>
		public void CancelReservation(int reservationId)
		{
			var reservation = context.Reservations.Where(r => r.Id == reservationId).FirstOrDefault();

			if (reservation == null)
				throw new NoSuchReservationException($"Rezewacja o id: '{reservationId}' nie istnieje!");

			context.Remove(reservation);
			context.SaveChanges();
		}


		/// <summary>
		///		Pobiera seans o podanym id
		/// </summary>
		public string GetReservations(int playingId)
		{
			var playing = context.Playings.Include(m => m.Movie).Include(h => h.Hall)
				.FirstOrDefault(p => p.Id == playingId);

			if (playing == null)
				throw new NoSuchPlayingException();

			StringBuilder sb = new StringBuilder();

			foreach (var item in playing.Reservations)
			{
				sb.Append(item.Seats);
			}

			return sb.ToString();
		}


		/// <summary>
		///		Pobiera listę repertuarów
		/// </summary>
		public List<Playing> GetRepertoire()
		{
			var playings = context.Playings
				.Include(m => m.Movie)
				.Include(h => h.Hall)
				.ToList();

			foreach (var item in playings)
			{
				context.Entry(item).Reference(m => m.Movie).Load();
				context.Entry(item).Reference(m => m.Hall).Load();
			}

			return playings;
		}


		/// <summary>
		///		Pobiera obraz dla filmu o podanym id
		/// </summary>
		public string GetMovieImage(int movieId)
		{
			var movie = context.Movies.FirstOrDefault(m => m.Id == movieId);

			if (movie == null)
				throw new NoSuchMovieException();

			byte[] imageBytes = File.ReadAllBytes(movie.ImagePath);

			string base64String = Convert.ToBase64String(imageBytes);

			return base64String;
		}


		/// <summary>
		///		Sprawdza czy można dokonać podanej rezerwacji dla danego seansu
		/// </summary>
		/// <param name="playingId"></param>
		/// <param name="seats"></param>
		/// <returns>
		///		-1 - błąd
		///		0 - można dokonać rezerwacji
		///		int > 0 - numer miejsca którego nie mazna zarezerwować
		/// </returns>
		public int CanMakeReservation(int playingId, string seats)
		{
			var playing = context.Playings
				.Include(m => m.Movie)
				.Include(h => h.Hall)
				.Include(r => r.Reservations)
				.FirstOrDefault(p => p.Id == playingId);

			context.Entry(playing).Reference(m => m.Movie).Load();
			context.Entry(playing).Reference(h => h.Hall).Load();
			context.Entry(playing).Collection(r => r.Reservations).Load();

			if (playing == null)
				return -1;

			var seatsSplitted = seats.Split(',');

			foreach (var item in seatsSplitted)
			{
				var seatNum = int.Parse(item);

				if (seatNum < 0 || seatNum > playing.Hall.Type.GetHashCode())
					return seatNum;
			}

			var result = playing.CanMakeReservation(seats);

			if (result == "0")
			{
				return 0;
			}
			else
			{
				return int.Parse(result.Split(",")[0]);
			}
		}


		/// <summary>
		///		Sprawdza czy można dokonać podanej rezerwacji dla danego seansu
		/// </summary>
		/// <param name="reservationId"></param>
		/// <param name="playingId"></param>
		/// <param name="seats"></param>
		/// <returns>
		///		-1 - błąd
		///		0 - można dokonać rezerwacji
		///		int > 0 - numer miejsca którego nie mazna zarezerwować
		/// </returns>
		public int CanEditReservation(int reservationId, int playingId, string seats)
		{
			var playing = context.Playings
				.Include(m => m.Movie)
				.Include(h => h.Hall)
				.Include(r => r.Reservations)
				.FirstOrDefault(p => p.Id == playingId);

			if (playing == null)
				return -1;

			var seatsSplitted = seats.Split(',');

			foreach (var item in seatsSplitted)
			{
				var seatNum = int.Parse(item);

				if (seatNum < 0 || seatNum > playing.Hall.Type.GetHashCode())
					return seatNum;
			}

			var result = playing.CanMakeReservation(reservationId, seats);

			if (result == "0")
			{
				return 0;
			}
			else
			{
				return int.Parse(result.Split(",")[0]);
			}
		}


		/// <summary>
		///		Zwraca obiekt seansu wraz ze wszystkimi szczegółami
		/// </summary>
		/// <param name="playingId">Id seansu</param>
		public Playing GetPlayingDetails(int playingId)
		{
			var playing = context.Playings
				.Include(m => m.Movie)
				.Include(h => h.Hall)
				.Include(r => r.Reservations)
				.FirstOrDefault(p => p.Id == playingId);

			if (playing == null)
				return null;

			var reservations = string.Empty;
			foreach (var item in playing.Reservations!)
			{
				reservations += item.Seats + ",";
			}

			if (playing.Reservations.Count > 0)
				reservations = reservations.Remove(reservations.Length - 1);

			playing.Movie.Image = GetMovieImage(playing.MovieId);
			playing.ActualReservations = reservations;
			playing.Reservations = null;

			return playing;
		}


		/// <summary>
		///		Zwraca wszystkie aktualne rezerwacje użytkownika
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public List<Reservation> GetAllUserReservation(int userId)
		{
			var reservations = context.Reservations
				.Include(p => p.Playing).ThenInclude(m => m.Movie)
				.Include(p => p.Playing).ThenInclude(m => m.Hall)
				.Where(r => r.UserId == userId)
				.ToList();

			foreach (var item in reservations)
			{
				item.Playing!.Reservations = null;
				item.Playing.Movie.Image = GetMovieImage(item.Playing.MovieId);
			}

			return reservations;
		}


		/// <summary>
		///		Zwraca rezerwacje użtrkownika dla seansu o podanym Id
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="playingId"></param>
		/// <returns></returns>
		public List<Reservation> GetUserReservation(int userId, int playingId)
		{
			var reservations = context.Reservations
				.Include(p => p.Playing).ThenInclude(m => m.Movie)
				.Include(h => h.Playing).ThenInclude(h => h.Hall)
				.Include(h => h.Playing).AsNoTracking()
				.Where(r => r.UserId == userId && r.PlayingId == playingId && r.Playing!.Date >= DateTime.Now)
				.ToList();

			foreach (var item in reservations)
			{
				item.Playing!.Reservations = null;
			}

			return reservations;
		}


		/// <summary>
		///		Zwraca konkretną rezerwację
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="reservationId"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public Reservation GetReservation(int userId, int reservationId)
		{
			try
			{
				var reservation = context.Reservations
					.Include(p => p.Playing).ThenInclude(m => m.Movie)
					.Include(h => h.Playing).ThenInclude(h => h.Hall)
					.Include(h => h.Playing).AsNoTracking()
					.Where(r => r.UserId == userId && r.Id == reservationId)
					.FirstOrDefault();

				reservation.Playing.Reservations = null;

				return reservation;
			}
			catch
			{
                Console.WriteLine($"Błąd podczas pobierania rezerwacji: {reservationId}");
				throw new InvalidOperationException("Brak danych");
            }
		}


		/// <summary>
		///		Edycja rezerwacji biletu
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="playingId"></param>
		/// <param name="reservationId"></param>
		/// <param name="seats"></param>
		/// <exception cref="MakeReservationException"></exception>
		/// <exception cref="InvalidDataException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public void EditReservation(int userId, int playingId, int reservationId, string seats)
		{
			User user = null;
			Playing playing = null;

			try
			{
				user = context.Users.First(u => u.Id == userId);
				playing = context.Playings.First(p => p.Id == playingId);
			}
			catch { throw new MakeReservationException(); }

			if (string.IsNullOrEmpty(seats) || Regex.IsMatch(seats, "[a-zA-Z]+"))
				throw new MakeReservationException();

			if (playing.CanMakeReservation(reservationId, seats) != "0")
				throw new MakeReservationException("Wybrane miejsca są już zajęte");


			try
			{
				var reservation = context.Reservations
					.Where(r => r.UserId == userId && r.Id == reservationId && r.PlayingId == playingId)
					.FirstOrDefault();

				if (reservation == null)
				{
					throw new InvalidDataException("W systemie nie ma takiej rezerwacji!");
				}

				var seatCount = seats.Split(',').Length;
				
				reservation.Date = DateTime.Now;
				reservation.Seats = seats;
				reservation.SeatCount = seatCount;
				reservation.ReservationCost = seatCount * reservation.Playing!.TicketCost;

				context.Update(reservation);
				context.SaveChanges();

				return;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Błąd podczas pobierania rezerwacji: {reservationId}");
				throw new InvalidOperationException($"Błąd podczas pobierania rezerwacji: {reservationId}, {ex.Message}");
			}
		}
	}
}
