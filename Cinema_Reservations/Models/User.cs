using System.ComponentModel.DataAnnotations;

namespace Cinema_Reservations.Models
{
	[Serializable]
	public class User
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }

		public User()
		{
			Name = string.Empty;
			Email = string.Empty;
			Password = string.Empty;
		}

		public User(int id, string name, string email, string password)
		{
			Id = id;
			Name = name;
			Email = email;
			Password = password;
		}
	}
}
