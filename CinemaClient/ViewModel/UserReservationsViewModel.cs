using CinemaClient.Model;
using CinemaServiceRef;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public UserReservationsViewModel()
        {
			CinemaService = new CinemaServiceClient();
			UserService = new UserServiceClient();
			UserReservations = new ObservableCollection<Reservation>();

			ShowPlayingCommand = new RelayCommand(ShowPlaying);
			CahngeReservationCommand = new RelayCommand(ChangeReservation);
		}

		private void ChangeReservation(object obj)
		{
			throw new NotImplementedException();
		}

		private void ShowPlaying(object obj)
		{
			var reservation = obj as Reservation;
			if (reservation != null)
			{
				CinemaPlayingWindow playing_details = new CinemaPlayingWindow(reservation.Playing, new CinemaPlayingViewModel(reservation.Playing));

				playing_details.ShowDialog();
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
