using System.Windows;
using UserServiceRef;

namespace CinemaClient
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private IUserService userService;

		public MainWindow()
		{
			userService = new UserServiceClient();
			InitializeComponent();
		}

		private async void registerBtn_Click(object sender, RoutedEventArgs e)
		{
			var name =		registerNameTB.Text;
			var email =		registerEmailTB.Text;
			var password =	registerPasswordTB.Text;

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
				var userId = await userService.AuthenticateUserAsync(new AuthenticateUserRequest()
				{
					Body = new AuthenticateUserRequestBody()
					{
						email = email,
						password = password
					}
				});

				App.SetUserId(userId);
			}
			catch (Exception ex)
			{
				LoginResponseTextBox.Text = $"Niepoprawne dane logowania! {ex.Message}";
				return;
			}

			mainWindow.Visibility = Visibility.Collapsed;

			CinemaWindow cinemaWindow = new CinemaWindow();
			cinemaWindow.ShowDialog();

			mainWindow.Visibility = Visibility.Visible;
		}
	}
}