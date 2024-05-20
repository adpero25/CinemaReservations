using CinemaClient.Model;
using CinemaServiceRef;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using UserServiceRef;
using Playing = CinemaServiceRef.Playing;
using Reservation = CinemaServiceRef.Reservation;
using iText.IO.Image;

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
		private UserServiceClient userService;

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
			userService = new UserServiceClient();
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

								var userDetails = (await userService.GetUserDataAsync(App.UserID)).Body.GetUserDataResult;
								var fileName = $"rezerwacja_{App.UserID}_{playing.Id}_{playing.Date.ToFileTime()}.pdf";

								// Initialize PDF writer
								PdfWriter writer = new PdfWriter(fileName);

								// Initialize PDF document
								PdfDocument pdf = new PdfDocument(writer);

								// Initialize document
								Document document = new Document(pdf);

								// Create a font
								PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
								PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

								// Create a table with 2 columns
								Table mainTable = new Table(UnitValue.CreatePercentArray(new float[] { 2, 2 })).UseAllAvailableWidth();
								Table contentTable = new Table(UnitValue.CreatePercentArray(new float[] { 1 })).UseAllAvailableWidth();

								contentTable.AddCell(new Cell().Add(new Paragraph($"Potwierdzenie rezerwacji biletu").SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER).SetFont(boldFont).SetFontSize(25)));
								contentTable.AddCell(new Cell().Add(new Paragraph($"{Environment.NewLine}")));
								contentTable.AddCell(new Cell().Add(new Paragraph($"Dane zamawiającego:").SetFont(boldFont).SetFontSize(20)));
								contentTable.AddCell(new Cell().Add(new Paragraph($"{userDetails.Name}").SetFont(font).SetItalic()));
								contentTable.AddCell(new Cell().Add(new Paragraph($"{userDetails.Email}").SetFont(font).SetItalic()));
								contentTable.AddCell(new Cell().Add(new Paragraph($"{Environment.NewLine}")));
								contentTable.AddCell(new Cell().Add(new Paragraph().Add(new Text("Seans: ").SetFont(boldFont)).Add(new Text($"{playing.Movie.Name}")).SetFont(font)));
								contentTable.AddCell(new Cell().Add(new Paragraph().Add(new Text($"Data: ").SetFont(boldFont)).Add(new Text($"{playing.Date}")).SetFont(font)));
								contentTable.AddCell(new Cell().Add(new Paragraph().Add(new Text($"Sala: ").SetFont(boldFont)).Add(new Text($"{playing.Hall.Number}")).SetFont(font)));
								contentTable.AddCell(new Cell().Add(new Paragraph().Add(new Text($"Miejsca: ").SetFont(boldFont)).Add(new Text($"{seats}")).SetFont(font)));
								contentTable.AddCell(new Cell().Add(new Paragraph().Add(new Text($"Do zapłaty: ").SetFont(boldFont)).Add(new Text($"{playing.TicketCost * userReservedPlaces.Count}pln")).SetFont(font)));
								contentTable.AddCell(new Cell().Add(new Paragraph($"{Environment.NewLine}")));
								contentTable.AddCell(new Cell().Add(new Paragraph($"Do zobaczenia na miejscu!").SetFont(boldFont)));

								mainTable.AddCell(new Cell().Add(contentTable));

								var img = new Image(ImageDataFactory.Create(Convert.FromBase64String(playing.Movie.Image)));
								img.SetAutoScale(true);

								mainTable.AddCell(new Cell().Add(img));
								
								document.Add(mainTable);

								// Close document
								document.Close();

								MessageBox.Show($"Rezerwacja udana!!!{Environment.NewLine}" +
									$"Życzymy udanego seansu!{Environment.NewLine}" +
									$"Potwierdzenie wygenerowane do pliku: {Environment.NewLine}" +
									$"{fileName}", "Sukces!", MessageBoxButton.OK, MessageBoxImage.Information);

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
