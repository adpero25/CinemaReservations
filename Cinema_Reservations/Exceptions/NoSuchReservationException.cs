using System.Runtime.Serialization;

namespace Cinema_Reservations.Exceptions
{
	[Serializable]
	public class NoSuchReservationException : Exception
	{
		public NoSuchReservationException() : base("Nie takiej rezerwacji!")
		{
		}

		public NoSuchReservationException(string? message) : base(message)
		{
		}

		public NoSuchReservationException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected NoSuchReservationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
