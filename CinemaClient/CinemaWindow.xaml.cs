using CinemaClient.ViewModel;
using System.Windows;

namespace CinemaClient
{
	/// <summary>
	/// Interaction logic for CinemaWindow.xaml
	/// </summary>
	public partial class CinemaWindow : Window
    {
		private CinemaViewModel viewModel;

        public CinemaWindow()
        {
			viewModel = new CinemaViewModel();
            DataContext = viewModel;
            InitializeComponent();
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			viewModel.UpdateReprtoire();
			viewModel.GetUserDetailsAsync();
		}
	}
}
