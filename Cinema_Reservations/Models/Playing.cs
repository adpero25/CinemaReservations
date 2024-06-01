using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_Reservations.Models
{
	/// <summary>
	///		Klasa opisująca pojedynczy element repertuaru kina wraz z rezerwacjami
	/// </summary>
	[Serializable]
	public class Playing
	{
		public int Id { get; set; }
		
		[Required]
		[ForeignKey("CinemaHalls")]
		public int HallId { get; set; }			// id sali kinowej
		public CinemaHall Hall { get; set; }
		
		[Required]
		[ForeignKey("Movies")]
		public int MovieId { get; set; }        // id filmu
		public Movie Movie { get; set; }
		
		[Required]
		public DateTime Date { get; set; }		// data seansu
		
		[Required]
		public PlayingState State { get; set; }	// stan seansu
		
		[Required]
		public float TicketCost { get; set; }   // koszt biletu wejściowego

		public List<Reservation>? Reservations { get; set; } = new List<Reservation>(); // lista rezerwacji

		[NotMapped]
		public string ActualReservations { get; set; } = string.Empty;

		[NotMapped]
		public bool HasFreeReservations 
		{
			get
			{
				var availableSpots = Hall.Type.GetHashCode();

                if (Reservations == null || Reservations.Count == 0)
					return true;

                foreach (Reservation reservation in Reservations)
					availableSpots -= reservation.SeatCount;				

				return availableSpots > 0;
			}
		}

		public Playing()
		{
			Movie = new Movie();
			Hall = new CinemaHall();
		}

        public Playing(int id, CinemaHall hall, Movie movie, DateTime date, PlayingState state, float ticketCost, List<Reservation>? reservations)
		{
			Id = id;
			Hall = hall;
			Movie = movie;
			Date = date;
			State = state;
			TicketCost = ticketCost;
			Reservations = reservations;
		}

		public string CanMakeReservation(string seats)
		{
			string[] seatsSeparated = seats.Split(",");

			if (Reservations == null || Reservations.Count == 0)
				return "0";

			foreach (var item in Reservations)
			{
				var itemSeats = item.Seats.Split(",");

				if (itemSeats.Intersect(seatsSeparated).Any())
					return string.Join(",", itemSeats.Intersect(seatsSeparated));
			}
			
			return "0";	
		}

		public string CanMakeReservation(int reservationId, string seats)
		{
			string[] seatsSeparated = seats.Split(",");

			if (Reservations == null || Reservations.Count == 0)
				return "0";

			foreach (var item in Reservations)
			{
				var itemSeats = item.Seats.Split(",");

				if (itemSeats.Intersect(seatsSeparated).Any() && item.Id != reservationId)
					return string.Join(",", itemSeats.Intersect(seatsSeparated));
			}

			return "0";
		}
	}

	public enum PlayingState
	{
		ComingSoon,
		Playing,
		Ended
	}
}
