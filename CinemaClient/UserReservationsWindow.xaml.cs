using CinemaClient.ViewModel;
using CinemaServiceRef;
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
using System.Windows.Shapes;
using UserServiceRef;

namespace CinemaClient
{
	/// <summary>
	/// Interaction logic for UserReservationsWindow.xaml
	/// </summary>
	public partial class UserReservationsWindow : Window
	{
		private UserReservationsViewModel viewModel;

		public UserReservationsWindow()
		{
			viewModel = new UserReservationsViewModel();
			DataContext = viewModel;
			InitializeComponent();
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			viewModel.GetUserReservations();
		}
	}
}
