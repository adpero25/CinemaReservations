using CinemaClient.Model;
using CinemaServiceRef;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UserServiceRef;

namespace CinemaClient.ViewModel
{
	public class UserReservationsViewModel : PropertyChangeModel
	{
		public ICinemaService CinemaService { get; }
		public IUserService UserService { get; }

		public ObservableCollection<Reservation> UserReservations { get; set; }
		
		public Reservation selectedReservation;
		public Reservation SelectedReservation 
		{
			get { return selectedReservation; } 
			set { selectedReservation = value; OnPropertyChange(); }
		}

		public ICommand ShowPlayingCommand { get; set; }
		public ICommand CahngeReservationCommand { get; set; }
		public ICommand CancelReservationCommand { get; set; }

		public UserReservationsViewModel()
        {
			CinemaService = new CinemaServiceClient();
			UserService = new UserServiceClient();
			UserReservations = new ObservableCollection<Reservation>();

			ShowPlayingCommand = new RelayCommand(ShowPlaying);
			CahngeReservationCommand = new AsyncRelayCommand(ChangeReservationAsync);
			CancelReservationCommand = new AsyncRelayCommand(CancelReservationAsync);
		}

		private async Task CancelReservationAsync(object arg)
		{
			var reservation = arg as Reservation;

			var reservationInfo = new StringBuilder();

			reservationInfo.AppendLine($"Seans: {reservation.Playing.Movie.Name}");
			reservationInfo.AppendLine($"Data seansu: {reservation.Playing.Date}");
			reservationInfo.AppendLine($"Data rezerwacji: {reservation.Date}");
			reservationInfo.AppendLine($"Miejsca: {reservation.Seats}");

			var result = MessageBox.Show($"Czy napewno chcesz odwołać rezerwację:\n{reservationInfo.ToString()}",
				"Wymagane potwierdzenie", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

			if (result == MessageBoxResult.Yes)
			{
				try
				{
					await CinemaService.CancelReservationAsync(reservation.Id);
					MessageBox.Show($"Operacja zakończona powodzeniem!\n" +
						$"Rezerwacja została odwołana.\n" +
						$"Do zobaczenia na innym seansie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

					UserReservations.Remove(reservation);
					OnPropertyChange(nameof(UserReservations));
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Operacja zakończona niepowodzeniem!\n{ex.Message}\n\nSpróbuj ponownie za chwilę.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private async Task ChangeReservationAsync(object obj)
		{
			var reservation = obj as Reservation;

			if (reservation != null)
			{
				if (reservation.Playing == null)
				{
					try
					{
						reservation.Playing = await CinemaService.GetPlayingDetailsAsync(reservation.PlayingId);
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Nie udało się pobrać danych dotyczących spektaklu :<\n{ex.Message}");
						return;
					}
				}

				Window? reservWindow = App.GetActiveWindow();

				CinemaPlayingWindow playing_details = new CinemaPlayingWindow(reservation.Playing, new CinemaPlayingViewModel(reservation.Playing), true, reservation.Id);
				
				if (reservWindow != null)
				{
					reservWindow.Visibility = Visibility.Collapsed;
				}

				playing_details.ShowDialog();

				if (reservWindow != null)
				{
					reservWindow.Visibility = Visibility.Visible;
				}
			}
		}

		private void ShowPlaying(object obj)
		{
			var reservation = obj as Reservation;
			if (reservation != null)
			{
				Window? reservWindow = App.GetActiveWindow();

				CinemaPlayingWindow playing_details = new CinemaPlayingWindow(reservation.Playing, new CinemaPlayingViewModel(reservation.Playing));

				if (reservWindow != null)
				{
					reservWindow.Visibility = Visibility.Collapsed;
				}

				playing_details.ShowDialog();

				if (reservWindow != null)
				{
					reservWindow.Visibility = Visibility.Visible;
				}
			}
		}

		public async void GetUserReservations()
		{
			var reservationsRequest = await CinemaService.GetAllUserReservationAsync(new GetAllUserReservationRequest()
			{
				userId = App.UserID,
			});

			var reservations = reservationsRequest.GetAllUserReservationResult;

			if (UserReservations == null || UserReservations.Count > 0)
				UserReservations = new ObservableCollection<Reservation>();

			foreach (var item in reservations)
			{
				UserReservations.Add(item);
			}
		}
	}
}
