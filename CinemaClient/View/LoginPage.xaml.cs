using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserServiceRef;

namespace CinemaClient.View
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
		private IUserService userService;
        
		public LoginPage()
        {
			userService = new UserServiceClient();

			InitializeComponent();
        }

		private async void registerBtn_Click(object sender, RoutedEventArgs e)
		{
			var name = registerNameTB.Text;
			var email = registerEmailTB.Text;
			var password = registerPasswordTB.Text;

			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				RegisterResponseTextBox.Text = "Niepoprawne dane";
			}

			try
			{
				await userService.AddUserAsync(new AddUserRequest()
				{
					Body = new AddUserRequestBody()
					{
						email = email,
						name = name,
						password = password
					}
				});
				registerNameTB.Text = null;
				registerEmailTB.Text = null;
				registerPasswordTB.Text = null;
			}
			catch (Exception ex)
			{
				RegisterResponseTextBox.Text = $"Nie udało się zarejestrować użytkownika! {ex.Message}";
				return;
			}

			RegisterResponseTextBox.Text = $"Zarejestrowano nowego użytkownika!\n" +
				$"Zaloguj się aby kontynuować";
		}

		private async void loginBtn_Click(object sender, RoutedEventArgs e)
		{
			var email = loginEmailTB.Text;
			var password = loginPasswordTB.Text;

			try
			{
				await userService.AuthenticateUserAsync(new AuthenticateUserRequest()
				{
					Body = new AuthenticateUserRequestBody()
					{
						email = email,
						password = password
					}
				});
			}
			catch (Exception ex)
			{
				LoginResponseTextBox.Text = $"Niepoprawne dane logowania! {ex.Message}";
				return;
			}


		}
	}
}
