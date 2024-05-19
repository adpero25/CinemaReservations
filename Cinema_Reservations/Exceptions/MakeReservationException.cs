using System.Runtime.Serialization;

namespace Cinema_Reservations.Exceptions
{
	[Serializable]
	public class MakeReservationException : Exception
	{
		public MakeReservationException() : base ("Nie udało się dokonać rezerwacji")
		{
		}

		public MakeReservationException(string? message) : base(message)
		{
		}

		public MakeReservationException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected MakeReservationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
