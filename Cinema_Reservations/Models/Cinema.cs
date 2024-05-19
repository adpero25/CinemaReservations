using System.ComponentModel.DataAnnotations;

namespace Cinema_Reservations.Models
{
	[Serializable]
	public class Cinema
	{
		public int Id { get; set; }
		
		[Required]
		public string Name { get; set; }

		[Required]
		public string Address { get; set; }

		public List<CinemaHall>? Halls { get; set; }
		
		public List<Playing>? Playings { get; set; }

		public Cinema()
		{
			Name = string.Empty;
			Address = string.Empty;
			Halls = new List<CinemaHall>();
			Playings = new List<Playing>();
		}

		public Cinema(int id, string name, string address, List<CinemaHall>? halls, List<Playing>? playings)
		{
			Id = id;
			Name = name;
			Address = address;
			Halls = halls;
			Playings = playings;
		}
    }
}
