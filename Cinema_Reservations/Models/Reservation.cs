using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;

namespace Cinema_Reservations.Models
{
	[Serializable]
	public class Reservation
	{
		public int Id { get; set; }
		
		[Required]
		[ForeignKey("Users")]
		public int UserId { get; set; }		// id uzytkownika dokonującego rezerwacji
		public virtual User? User { get; set; }
		
		[Required]
		[ForeignKey("Playings")]
		public int PlayingId { get; set; }	// id pojedynczego seansu
		public virtual Playing? Playing { get; set; }

		[Required]
		public DateTime Date { get; set; }	// data rezerwacji

		[Required]
		public float ReservationCost {  get; set; }	// całkowity koszt rezerwacji
		
		[Required]
		public int SeatCount { get; set; }	// liczba zajętych miejsc
		
		[Required]
		public string Seats { get; set; }   // numery miejsc oddzielone przecinkami

		//public bool IsCanceled { get; set; } = false;	// flaga czy rezerwacja została odwołana

		public Reservation()
		{ }

		public Reservation(int id, User user, Playing playing, DateTime date, float reservationCost, int seatCount, string seats)
		{
			Id = id;
			User = user;
			Playing = playing;
			Date = date;
			ReservationCost = reservationCost;
			SeatCount = seatCount;
			Seats = seats;
		}
	}
}
