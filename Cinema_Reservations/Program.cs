using Cinema_Reservations.DAL;
using Cinema_Reservations.WS;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using SoapCore;
using System.ServiceModel.Channels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

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

			Host.CreateDefaultBuilder()
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.ConfigureKestrel(serverOptions =>
					{
						serverOptions.Listen(IPAddress.Any, 5000); // HTTP
						serverOptions.Listen(IPAddress.Any, 5001, listenOptions =>
						{
							listenOptions.UseHttps($"{Path.Combine(Environment.CurrentDirectory, Path.Combine("cert", "aspnetcore.pfx"))}", "pass");
						});
					});
				});



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