using System.Runtime.Serialization;

namespace Cinema_Reservations.Exceptions
{
	[Serializable]
	public class NoSuchPlayingException : Exception
	{
		public NoSuchPlayingException() : base("Nie ma takiego seansu")
		{
		}

		public NoSuchPlayingException(string? message) : base(message)
		{
		}

		public NoSuchPlayingException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected NoSuchPlayingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
