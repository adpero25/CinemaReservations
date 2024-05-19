using System.Runtime.Serialization;

namespace Cinema_Reservations.Exceptions
{
	[Serializable]
	public class AuthenticationException : Exception
	{
		public AuthenticationException() : base("Błąd autentykacji") 
		{
		}

		public AuthenticationException(string? message) : base(message)
		{
		}

		public AuthenticationException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
