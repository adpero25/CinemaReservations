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

namespace CinemaClient.ViewModel
{
	public enum State
	{
		None,
		Reserve
	}

	public class CinemaPlayingViewModel : PropertyChangeModel
	{
		private CinemaServiceClient cinemaService;

		private Playing playing;
		public Playing Playing
		{
			get
			{
				return playing;
			}
			set
			{
				playing = value;
				OnPropertyChange(nameof(Playing));
			}
		}

		private State currentState = State.None;
		public State CurrentState
		{
			get
			{
				return currentState;
			}

			set
			{
				currentState = value;
				OnPropertyChange(nameof(CurrentState));
				OnPropertyChange(nameof(ReserveButtonTekst));
				OnPropertyChange(nameof(ScreenInfoDisplay));
				StateChanged?.Invoke(this, new EventArgs());
			}
		}

		public string ReserveButtonTekst
		{
			get { return currentState == State.None ? "Zarezerwuj bilet" : "OK"; }
		}

		public Visibility ScreenInfoDisplay
		{
			get { return currentState == State.None ? Visibility.Collapsed : Visibility.Visible; }
		}

		public ObservableCollection<string> userReservedPlaces { get; set; }
		public List<Reservation> userReservations { get; set; }

		public EventHandler StateChanged { get; set; }

		public ICommand ReserveCommand { get; set; }

		public CinemaPlayingViewModel(Playing _playing)
		{
			cinemaService = new CinemaServiceClient();
			userReservedPlaces = new ObservableCollection<string>();
			ReserveCommand = new AsyncRelayCommand(ReservePlacesCommandAsync);
			Playing = _playing;
		}

		public async Task DownloadPlayingDetails()
		{
			try
			{
				var play = await cinemaService.GetPlayingDetailsAsync(Playing.Id);

				if (play != null)
				{
					Playing = play;
				}
				else
				{
					throw new Exception("Nie udało się pobrać danych z serwera!");
				}
			}
			catch (Exception ex) 
			{
				MessageBox.Show($"Nie udało się pobrać danych seansu: {ex.Message}");
				throw;
			}
		}

		private async Task ReservePlacesCommandAsync(object sender)
		{
			if (currentState == State.None)
			{
				CurrentState = State.Reserve;
				return;
			}

			if (CurrentState == State.Reserve && userReservedPlaces.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				var caption = string.Empty;
				var seats = string.Join(", ", userReservedPlaces);

				if (userReservedPlaces.Count == 1)
					caption = $"Rezerwacja biletu na seans: {Playing.Movie.Name}";
				else
					caption = $"Rezerwacja biletów na seans: {Playing.Movie.Name}";

				stringBuilder.AppendLine(caption);
				stringBuilder.AppendLine($"Data rezerwacji: {Playing.Date.ToString("f")}");
				stringBuilder.AppendLine($"Sala numer: {Playing.Hall.Number}");
				stringBuilder.AppendLine($"Liczba miejsc: {userReservedPlaces.Count}");
				stringBuilder.AppendLine($"Zarezerwowane miejsca: {seats}");
				stringBuilder.AppendLine($"Cena:{Environment.NewLine}\t{userReservedPlaces.Count} * {Playing.TicketCost}PLN = {userReservedPlaces.Count * Playing.TicketCost}PLN");

				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Czy chcesz dokonać rezerwacji?");

				var mbResult = MessageBox.Show(stringBuilder.ToString(), caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

				if (mbResult == MessageBoxResult.Yes)
				{
					try
					{
						var result = await cinemaService.CanMakeReservationAsync(playing.Id, seats);
						if (result == 0)
						{
							try
							{
								await cinemaService.MakeReservationAsync(DateTime.Now, App.UserID, playing.Id, seats);

								MessageBox.Show($"Rezerwacja udana!!!{Environment.NewLine}Życzymy udanego seansu!", "Sukces!", MessageBoxButton.OK, MessageBoxImage.Information);

								CurrentState = State.None;
							}
							catch (Exception ex)
							{
								MessageBox.Show($"Nie udało się dokonać rezerwacji: {ex.Message}", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Warning);
							}
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Nie udało się sprawdzić rezerwacji: {ex.Message}", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Warning);
					}

				}

				userReservedPlaces.Clear();
			}

			CurrentState = State.None;
		}

		public async Task DownloadUserReservationsForPlaying()
		{
			try
			{
				var reservations = await cinemaService.GetUserReservationAsync(App.UserID, Playing.Id);

				userReservations = reservations.GetUserReservationResult.ToList();
			}
			catch (Exception e)
			{
				MessageBox.Show($"Nie udało się pobrać Twoich rezerwacji: {e.Message}", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Warning);
				throw;
			}
		}
	}
}
