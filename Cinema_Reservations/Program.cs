using Cinema_Reservations.DAL;
using Cinema_Reservations.WS;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using SoapCore;
using System.ServiceModel.Channels;

namespace Cinema_Reservations
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDbContext<CinemaContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("CinemaCS")).EnableSensitiveDataLogging());


			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.Services.AddScoped<SeederContext>();
			builder.Services.AddScoped<ICinemaService, CinemaService>();
			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddSoapCore();

			var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				using var context = scope.ServiceProvider.GetRequiredService<CinemaContext>();
				context.Database.EnsureCreated();

				using var seedercontext = scope.ServiceProvider.GetRequiredService<SeederContext>();
				seedercontext.Seed();
				/* ADD SEEDER HERE */
			}

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.UseSoapEndpoint<ICinemaService>("/CinemaService", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
				endpoints.UseSoapEndpoint<IUserService>("/UserService", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
			});

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}