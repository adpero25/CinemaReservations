﻿using CinemaClient.Model;
using System.Configuration;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Windows;
using UserServiceRef;

namespace CinemaClient
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static int userID;
		public static int UserID
		{
			get { return userID; }
			private set
			{
				userID = value;
				new PropertyChangeModel().OnPropertyChange(nameof(UserID));
			}
		}
		private static User user;
		public static User User
		{
			get { return user; }
			private set
			{
				user = value;
				new PropertyChangeModel().OnPropertyChange(nameof(User));
			}
		}

		public static void SetUserId(AuthenticateUserResponse idResponse)
		{
			if (idResponse.Body.AuthenticateUserResult > 0)
			{
				UserID = idResponse.Body.AuthenticateUserResult;
				return;
			}

			throw new Exception("Niepoprawny ID użytkownika");
		}

		public static void SetUser(User user)
		{
			User = user;
		}

		public static Window? GetActiveWindow()
		{
			try
			{
				return Current.Windows.OfType<Window>().ToList()[Current.Windows.Count - 2];
			}
			catch (Exception ex)
			{
				return Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
			}

		}

	}

}
