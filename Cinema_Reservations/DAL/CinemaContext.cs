using Cinema_Reservations.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Cinema_Reservations.DAL
{
	public class CinemaContext : DbContext
	{
		public DbSet<Cinema> Cinemas { get; set; }
		public DbSet<CinemaHall> CinemaHalls { get; set; }
		public DbSet<Movie> Movies { get; set; }
		public DbSet<Playing> Playings { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
		public DbSet<User> Users { get; set; }

		public CinemaContext(DbContextOptions<CinemaContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>().HasData(new User()
			{
				Id = 1,
				Email = "admin@admin.pl",
				Name = "admin",
				Password = "admin"
			});
		}
	}
}
