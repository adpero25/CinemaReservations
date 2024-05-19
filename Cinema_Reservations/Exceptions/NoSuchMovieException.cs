using System.Runtime.Serialization;

namespace Cinema_Reservations.Exceptions
{
	[Serializable]
	public class NoSuchMovieException : Exception
	{
		public NoSuchMovieException() : base("Nie ma takiego filmu")
		{
		}

		public NoSuchMovieException(string? message) : base(message)
		{
		}

		public NoSuchMovieException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected NoSuchMovieException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
