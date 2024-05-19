using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_Reservations.Models
{
	[Serializable]
	public class Movie
	{
		public int Id { get; set; }
		
		[Required]
		public string Name { get; set; }
		
		[Required]
		public string Director {  get; set; }

		[Required]
		public string Script { get; set; }

		[Required]
		public string Genre { get; set; }

		[Required] 
		public float Grade { get; set; } // out of 10

		[Required]
		public TimeSpan Duration { get; set; }

		[Required]
		public int ProductionYear { get; set; }
		
		[MaxLength(500)]
		public string? DescriptionShort { get; set; }
		
		[MaxLength(2000)]
		public string? DescriptionLong { get; set; }
		public string? ImagePath { get; set; }

		[NotMapped]
		public string? Image { get; set; }

		public Movie()
		{
			Name = string.Empty;
			Director = string.Empty;
		}

		public Movie(int id, string name, string producer, int productionYear, string? descriptionShort, string? imagePath, string? descriptionLong, string genre, float grade, string script)
		{
			Id = id;
			Name = name;
			Director = producer;
			ProductionYear = productionYear;
			DescriptionShort = descriptionShort;
			DescriptionLong = descriptionLong;
			ImagePath = imagePath;
			Genre = genre;
			Grade = grade;
			Script = script;
		}
	}
}
