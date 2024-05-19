using CinemaClient.Model;
using CinemaClient.ViewModel;
using CinemaServiceRef;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace CinemaClient
{
	enum SeatColors
	{
		DarkCyan = 0,
		Red = 1,
		Gray = 2,
		Green = 3,
	}

	/// <summary>
	/// Interaction logic for CinemaPlayingWindow.xaml
	/// </summary>
	public partial class CinemaPlayingWindow : Window
	{
		public Playing Playing;
		private bool windowInitialized;
		private string[] reservedPlaces;
		private List<string> UserReservations;
		private CinemaPlayingViewModel viewModel;

		public CinemaPlayingWindow(Playing _playing, CinemaPlayingViewModel _playingViewModel)
		{
			viewModel = _playingViewModel;
			viewModel.StateChanged += ReservationStateChanged;
			Playing = _playing;
			DataContext = viewModel;
			windowInitialized = false;

			InitializeComponent();
		}

		private async void Window_Initialized(object sender, EventArgs e)
		{
			if (!(await DownoladDetailsAsync()))
				return;

			var hall = Playing.Hall;

			for (int i = 0; i < hall.Rows; i++)
			{
				RowDefinition row = new RowDefinition()
				{
					Height = new GridLength(1, GridUnitType.Star),
					MinHeight = 29.0
				};
				PlacesRow.RowDefinitions.Add(row);
			}

			for (int j = 0; j < hall.Columns / 2; j++)
			{
				var col = new ColumnDefinition()
				{
					Width = new GridLength(1, GridUnitType.Star)
				};
				PlacesRow.ColumnDefinitions.Add(col);
			}

			var centerCol = new ColumnDefinition()
			{
				Width = new GridLength(3, GridUnitType.Star)
			};
			PlacesRow.ColumnDefinitions.Add(centerCol);

			for (int j = 0; j < hall.Columns / 2; j++)
			{
				var col = new ColumnDefinition()
				{
					Width = new GridLength(1, GridUnitType.Star)
				};
				PlacesRow.ColumnDefinitions.Add(col);
			}

			await InitCinemaDisplayAsync();

			windowInitialized = true;
		}

		private async void ReservationStateChanged(object? sender, EventArgs e)
		{
			var children = PlacesRow.Children;

			foreach (var child in children)
			{
				if (child != null && child is Rectangle)
				{
					if ((child as Rectangle)!.Fill.ToString() == new SolidColorBrush(Colors.Gray).ToString())
					{
						(child as Rectangle)!.Fill = new SolidColorBrush(Colors.Green);
					}
				}
			}

			await InitCinemaDisplayAsync();
		}

		private async Task<bool> DownloadUserReservationsAsync()
		{
			try
			{
				await viewModel.DownloadUserReservationsForPlaying();

				UserReservations = new List<string>();

				viewModel.userReservations.ForEach(s => 
				{ 
					UserReservations.AddRange(s.Seats.Split(',').Select(p => p.Trim()).ToArray()); 
				});

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		private async Task<bool> DownoladDetailsAsync()
		{
			try
			{
				await viewModel.DownloadPlayingDetails();
				Playing = viewModel.Playing;

				var reservations = Playing.ActualReservations;
				var reservedPlacesSB = new StringBuilder();

				reservedPlaces = reservations.Split(',').Select(p => p.Trim()).ToArray();

				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Nie udało się pobrać szczegółów seansu: {ex.Message}",
					Name, MessageBoxButton.OK, MessageBoxImage.Error);

				return false;
			}
		}

		private async Task InitCinemaDisplayAsync()
		{
			if (!(await DownoladDetailsAsync()))
				return;

			if (!(await DownloadUserReservationsAsync()))
				return;

			PlacesRow.Children.Clear();

			var colReducer = 0;
			for (int row = 0; row < PlacesRow.RowDefinitions.Count; row++)
			{
				for (int col = 0; col < PlacesRow.ColumnDefinitions.Count; col++)
				{
					if (col == PlacesRow.ColumnDefinitions.Count / 2)
					{
						colReducer++;
						continue;
					}

					var seatNum = row * PlacesRow.ColumnDefinitions.Count + col + 1 - colReducer;

					Rectangle rectangle = new Rectangle();

					if (UserReservations.Intersect(new string[] { seatNum.ToString() }).Any())
						rectangle.Fill = new SolidColorBrush(Colors.DarkCyan);
					else if (reservedPlaces.Intersect(new string[] { seatNum.ToString() }).Any())
						rectangle.Fill = new SolidColorBrush(Colors.Red);
					else
						rectangle.Fill = new SolidColorBrush(Colors.Green);

					rectangle.Style = (Style)FindResource("RectangleWithTextStyle");
					rectangle.Name = $"rec_{seatNum}";
					rectangle.RadiusX = 10;
					rectangle.RadiusY = 8;
					rectangle.Width = 32;
					rectangle.Height = 22;
					rectangle.MouseLeave += UnMarkSeat;
					rectangle.MouseLeftButtonDown += SeatClick;

					TextBlock textBlock = new TextBlock();
					textBlock.FontSize = 10;
					textBlock.Text = (seatNum).ToString();
					textBlock.HorizontalAlignment = HorizontalAlignment.Center;
					textBlock.VerticalAlignment = VerticalAlignment.Center;
					textBlock.Foreground = new SolidColorBrush(Colors.White);

					Border border = new Border();
					border.BorderThickness = new Thickness(0.1);
					border.BorderBrush = new SolidColorBrush(Colors.White);

					Grid.SetRow(rectangle, row);
					Grid.SetColumn(rectangle, col);
					Grid.SetRow(textBlock, row);
					Grid.SetColumn(textBlock, col);
					Grid.SetRow(border, row);
					Grid.SetColumn(border, col);

					PlacesRow.Children.Add(border);
					PlacesRow.Children.Add(rectangle);
					PlacesRow.Children.Add(textBlock);
				}
			}
		}



		private void UnMarkSeat(object sender, MouseEventArgs e)
		{
			var rect = sender as Rectangle;

			if (rect.Fill.ToString() != new SolidColorBrush(Colors.Gray).ToString())
			{
				return;
			}

			if (viewModel.CurrentState == State.None)
				rect.Fill = new SolidColorBrush(Colors.Green);
		}

		private void SeatClick(object sender, MouseButtonEventArgs e)
		{
			var rect = sender as Rectangle;

			if (rect.Fill.ToString() == new SolidColorBrush(Colors.Red).ToString() ||
				rect.Fill.ToString() == new SolidColorBrush(Colors.DarkCyan).ToString())
			{
				return;
			}

			var seatNum = rect.Name.Split("_")[1];

			if (rect.Fill.ToString() == new SolidColorBrush(Colors.Gray).ToString())
			{
				if (viewModel.CurrentState == State.Reserve)
					if (viewModel.userReservedPlaces.FirstOrDefault(p => p == seatNum) != null)
						viewModel.userReservedPlaces.Remove(seatNum);

				rect.Fill = new SolidColorBrush(Colors.Green);
			}
			else
			{
				if (viewModel.CurrentState == State.Reserve)
					viewModel.userReservedPlaces.Add(seatNum);

				rect.Fill = new SolidColorBrush(Colors.Gray);
			}
		}
	}
}
