using CinemaClient.Model;
using CinemaServiceRef;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UserServiceRef;

namespace CinemaClient.ViewModel
{
    enum ContentWidth
    {
        ZeroStar = 0,
        SingleStar = 1,
        DoubleStar = 2,
        TripleStar = 3,
        FourtStar = 4,
        FifthStar = 5,
    }

    class CinemaViewModel : PropertyChangeModel
    {
		private ICinemaService cinemaService;
		private IUserService userService;

		public ObservableCollection<Playing> AvailablePlayings { get; set; }

        private BitmapImage selectedMovieImage;
        public BitmapImage SelectedMovieImage 
        { 
            get { return selectedMovieImage; } 
            set
            {
                selectedMovieImage = value;
                OnPropertyChange();
            }
        }

		private Playing selectedPlaying;
		public Playing SelectedPlaying 
        { 
            get {  return selectedPlaying; }
            set
            {
                var val = value;

                if (val == selectedPlaying)
                    selectedPlaying = null;
                else
                    selectedPlaying = value;

				OnPropertyChange();
                
                if (SelectedPlaying != null)
                    InfoPanelWidth = ContentWidth.DoubleStar.GetHashCode().ToString();
                else
                    InfoPanelWidth = ContentWidth.ZeroStar.GetHashCode().ToString();
			}
        }

        public string infoPanelWidth;
        public string InfoPanelWidth
        {
            get { return infoPanelWidth; }
            set 
            { 
                infoPanelWidth = string.Format($"{value}*");
                OnPropertyChange();
            }
        }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set { userName = value; OnPropertyChange(); }
        }

		private string userEmail;
		public string UserEmail
		{
			get { return userEmail; }
			set { userEmail = value; OnPropertyChange(); }
		}
        
		public ICommand ShowPlayingCommand {  get; set; }

		public CinemaViewModel()
        {
            cinemaService = new CinemaServiceClient();
			userService = new UserServiceClient();
			AvailablePlayings = new ObservableCollection<Playing>();
            InfoPanelWidth = "0";

			ShowPlayingCommand = new RelayCommand(ShowPlaying);
        }

		private void ShowPlaying(object obj)
		{
            var playing = obj as Playing;
            if (playing != null)
            {
			    CinemaPlayingWindow playing_details = new CinemaPlayingWindow(playing, new CinemaPlayingViewModel(playing));

			    playing_details.ShowDialog();
            }
		}

		public async void UpdateReprtoire()
        {
            var play = await cinemaService.GetRepertoireAsync(new GetRepertoireRequest());

            AvailablePlayings = new ObservableCollection<Playing>();

            foreach (var item in play.GetRepertoireResult)
            {
				AvailablePlayings.Add(item);
                item.Movie.Image = await cinemaService.GetMovieImageAsync(item.Movie.Id);
			}

            OnPropertyChange(nameof(AvailablePlayings));
		}

		private BitmapImage ConvertToBitmap(string img)
		{
			// Convert the image data to a byte array
			byte[] imageBytes = Convert.FromBase64String(img);

			// Convert the byte array to a BitmapImage and display it
			BitmapImage bitmapImage = new BitmapImage();
			using (MemoryStream stream = new MemoryStream(imageBytes))
			{
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = stream;
				bitmapImage.EndInit();
			}

            return bitmapImage;
		}

		public async void GetUserDetailsAsync()
		{
			var userDetails = (await userService.GetUserDataAsync(new GetUserDataRequest()
            {
                Body = new GetUserDataRequestBody()
                {
                    userId = App.UserID
				}
            })).Body.GetUserDataResult;

            App.SetUser(userDetails);

            UserName = userDetails.Name;
            UserEmail = userDetails.Email;
		}
	}
}
