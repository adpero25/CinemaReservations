using CinemaClient.Model;
using CinemaClient.ViewModel;
using CinemaServiceRef;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
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

		// do edycji z panelu listy rezerwacji
		public bool IsEditOperationEnable { get; private set; }
		public int EditedReservationId { get; private set; }

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
			IsEditOperationEnable = false;
			EditedReservationId = -1;

			InitializeComponent();
		}

		public  CinemaPlayingWindow(Playing _playing, CinemaPlayingViewModel _playingViewModel, bool isEditOperation = false, int reservationId = 0)
		{
			viewModel = _playingViewModel;
			viewModel.StateChanged += ReservationStateChanged;
			Playing = _playing;
			DataContext = viewModel;
			windowInitialized = false;
			IsEditOperationEnable = isEditOperation;
			EditedReservationId = reservationId;

			InitializeComponent();
		}

		private async void Window_Initialized(object sender, EventArgs e)
		{
			if (windowInitialized)
				return;

			if (!(await DownoladDetailsAsync()))
				return;
			
			DrawCinema();

			await InitCinemaDisplayAsync();

			windowInitialized = true;

			if (IsEditOperationEnable)
			{
				EditReservation(EditedReservationId);
			}
		}

		private void DrawCinema()
		{
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

			//var centerCol = new ColumnDefinition()
			//{
			//	Width = new GridLength(3, GridUnitType.Star)
			//};
			//PlacesRow.ColumnDefinitions.Add(centerCol);

			for (int j = 0; j < hall.Columns / 2; j++)
			{
				var col = new ColumnDefinition()
				{
					Width = new GridLength(1, GridUnitType.Star)
				};
				PlacesRow.ColumnDefinitions.Add(col);
			}
		}

		private async void ReservationStateChanged(object? sender, EventArgs e)
		{
			if (viewModel.CurrentState == State.Edit)
			{

			}
			else if (viewModel.CurrentState == State.Reserve)
			{

			}
			else //(viewModel.CurrentState == State.None)
			{
				var children = PlacesRow.Children;

				foreach (var child in children)
				{
					if (child != null && child is Rectangle)
					{
						if ((child as Rectangle)!.Fill.ToString() == Colors.Gray.ToString())
						{
							(child as Rectangle)!.Fill = new SolidColorBrush(Colors.Green);
						}
					}
				}

				await InitCinemaDisplayAsync();
			}
		}

		private async Task<bool> DownloadUserReservationsAsync()
		{
			try
			{
				await viewModel.DownloadUserReservationsForPlaying();

				UserReservations = new List<string>();

				viewModel.UserReservations.ToList().ForEach(s =>
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
					//if (col == PlacesRow.ColumnDefinitions.Count / 2)
					//{
					//	colReducer++;
					//	continue;
					//}

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
					rectangle.Tag = seatNum;
					rectangle.RadiusX = 10;
					rectangle.RadiusY = 2;
					rectangle.Width = 42;
					rectangle.Height = 24;
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

			if (rect.Fill.ToString() != Colors.Gray.ToString())
			{
				return;
			}

			if (viewModel.CurrentState == State.None)
				rect.Fill = new SolidColorBrush(Colors.Green);
		}

		private void SeatClick(object sender, MouseButtonEventArgs e)
		{
			var rect = sender as Rectangle;

			if (rect.Fill.ToString() == Colors.Red.ToString() ||
				rect.Fill.ToString() == Colors.DarkCyan.ToString())
			{
				return;
			}

			var seatNum = rect.Name.Split("_")[1];


			if (viewModel.CurrentState == State.None)
			{
				if (rect.Fill.ToString() == Colors.Gray.ToString())
				{
					rect.Fill = new SolidColorBrush(Colors.Green);
				}
				else
				{
					rect.Fill = new SolidColorBrush(Colors.Gray);
				}
			}
			else if (viewModel.CurrentState == State.Reserve || viewModel.CurrentState == State.Edit)
			{
				if (rect.Fill.ToString() == Colors.Gray.ToString() ||
					rect.Fill.ToString() == Colors.Blue.ToString())
				{
					viewModel.userReservedPlaces.Remove(seatNum);
					rect.Fill = new SolidColorBrush(Colors.Green);
				}
				else
				{
					viewModel.userReservedPlaces.Add(seatNum);
					rect.Fill = new SolidColorBrush(Colors.Gray);
				}
			}
		}

		private void list_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			MainSW.ScrollToVerticalOffset(MainSW.ContentVerticalOffset - e.Delta / 2);
		}

		private void EditButton_Click(object sender, RoutedEventArgs e)
		{
			var reservationId = (sender as Button).Tag;

			EditReservation((int)reservationId);
		}

		internal async void EditReservation(int reservationId)
		{
			Reservation? reservation;

			try
			{
				reservation = (await viewModel.GetReservation(reservationId)) as Reservation;

				if (reservation == null)
				{
					throw new InvalidOperationException("Brak danych dot. rezerwacji");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Brak zawartości do wyświetlenia", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var seats = reservation.Seats.Split(",").Select(s => s.Trim()).ToArray();

			foreach (var item in PlacesRow.Children)
			{
				if (item is Rectangle)
				{
					var rec = (item as Rectangle);

					if (rec.Fill.ToString() != Colors.Green.ToString())
					{
						rec.Fill = new SolidColorBrush(Colors.Red);
					}

					if (seats.Intersect(new string[] { rec.Tag.ToString() }).Any())
					{
						rec.Fill = new SolidColorBrush(Colors.Blue);
					}
				}
			}

			viewModel.CurrentState = State.Edit;
			viewModel.Reservation = reservation;
			viewModel.userReservedPlaces = new ObservableCollection<string>(seats);
		}

		private void reservePlaces_Click(object sender, RoutedEventArgs e)
		{
			MainSW.ScrollToBottom();
		}


		/// <summary>
		///		Podświetla siedzenia danej rezerwacji po kliknięciu na element listy
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// przywróć kolory domyslne
			foreach (var item in PlacesRow.Children)
			{
				if (item is Rectangle)
				{
					var rec = (item as Rectangle);

					if (rec.Fill.ToString() == Colors.Blue.ToString())
					{
						rec.Fill = new SolidColorBrush(Colors.DarkCyan);
					}
				}
			}

			var reservation = list.SelectedItem as Reservation;

			if (reservation == null) { return; }

			var seats = reservation.Seats.Split(",").Select(s => s.Trim()).ToList();

			foreach (var item in PlacesRow.Children)
			{
				if (item is Rectangle)
				{
					var rec = (item as Rectangle);

					if (seats.Intersect(new string[] { rec.Tag.ToString() }).Any())
					{
						rec.Fill = new SolidColorBrush(Colors.Blue);
					}
				}
			}
		}
	}
}
