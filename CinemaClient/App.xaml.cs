using System.Configuration;
using System.Data;
using System.Windows;
using UserServiceRef;

namespace CinemaClient
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{

		public static int UserID { get; private set; }

		public static void SetUserId(AuthenticateUserResponse idResponse)
		{
			if (idResponse.Body.AuthenticateUserResult > 0)
			{
				UserID = idResponse.Body.AuthenticateUserResult;
				return;
			}

			throw new Exception("Niepoprawny ID użytkownika");
		}
	}

}
