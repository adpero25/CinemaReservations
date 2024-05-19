using Cinema_Reservations.DAL;
using Cinema_Reservations.Exceptions;
using Cinema_Reservations.Models;
using System.Data.Entity;
using System.ServiceModel;

namespace Cinema_Reservations.WS
{
	[ServiceContract]
	public interface IUserService
	{
		[OperationContract]
		void AddUser(string name, string email, string password);
		[OperationContract]
		int AuthenticateUser(string email, string password);
	}

	public class UserService : IUserService
	{
		private readonly CinemaContext context;

		public UserService(CinemaContext _context)
        {
			context = _context;
		}


		public void AddUser(string name, string email, string password) 
		{
			var user = new User()
			{
				Email = email,
				Password = password,
				Name = name
			};
		
			var dbUser = context.Users.Where(u => u.Email == user.Email).FirstOrDefault();

			if (dbUser != null)
			{
				throw new UserAlreadyExistException();
			}

			context.Add(user);
			context.SaveChangesAsync();

			Console.WriteLine($"Dodano nowego użytkownika: {user.Name}: {user.Email}");
		}


		public int AuthenticateUser(string email, string password)
		{
			var user = context.Users.Single(u => u.Email == email);

			if (user == null) { throw new AuthenticationException("Nie ma takiego użytkownika"); }

			if (user.Password != password) { throw new AuthenticationException("Niepoprawny email lub hasło"); }

			Console.WriteLine($"Zalogowano: {user.Name}: {user.Email}");

			return user.Id;
		}
    }
}
