using System.Runtime.Serialization;

namespace Cinema_Reservations.Exceptions
{
	[Serializable]
	public class UserAlreadyExistException : Exception
	{
		public UserAlreadyExistException() : base("Taki użytkownik już istnieje")
		{
		}

		public UserAlreadyExistException(string? message) : base(message)
		{
		}

		public UserAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected UserAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
