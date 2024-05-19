
using Cinema_Reservations.Models;

namespace Cinema_Reservations.DAL
{
	public class SeederContext : IDisposable
	{
		private readonly CinemaContext context;

		public SeederContext(CinemaContext context)
		{
			this.context = context;
		}

		public void Dispose()
		{
		}

		public void Seed()
		{
			if (!context.Cinemas.Any())
			{
				context.Add(new Cinema
				{
					Name = "PBCinema",
					Address = "Wiejska 45A, Białystok"
				});

				context.SaveChanges();
			}


			if (!context.CinemaHalls.Any())
			{
				context.AddRange(new List<CinemaHall>()
				{
					new CinemaHall
					{
						Number = 1,
						Type = HallType.Small,
						Rows = HallType.Small.GetHashCode() / HallColumn.Columns.GetHashCode(),
						Columns = HallColumn.Columns.GetHashCode(),
					},
					new CinemaHall
					{
						Number = 2,
						Type = HallType.Medium,
						Rows = HallType.Medium.GetHashCode() / HallColumn.Columns.GetHashCode(),
						Columns = HallColumn.Columns.GetHashCode(),
					},
					new CinemaHall
					{
						Number = 3,
						Type = HallType.Large,
						Rows = HallType.Large.GetHashCode() / HallColumn.Columns.GetHashCode(),
						Columns = HallColumn.Columns.GetHashCode(),
					}
				});

				context.SaveChanges();
			}


			if (!context.Movies.Any())
			{
				context.AddRange(new List<Movie>()
				{
					new Movie
					{
						Name = "Interstellar",
						DescriptionShort = "Byt ludzkości na Ziemi dobiega końca wskutek zmian klimatycznych. Grupa naukowców odkrywa tunel czasoprzestrzenny, który umożliwia poszukiwanie nowego domu.",
						DescriptionLong = "Na skutek błędów popełnionych przez ludzkość w XX w. Ziemia staje na krawędzi zagłady. Nastąpił upadek państw, a ich rządy straciły władzę. Szczątkowo funkcjonująca gospodarka nie jest w stanie zapewnić ludziom nawet żywności. Gdy jednak odkryta zostaje możliwość podróżowania w czasoprzestrzeni, naukowcy wywodzący się z resztek organizacji NASA podejmują się jej zbadania, tym samym stając się ostatnią deską ratunku dla ludzi i ich planety.",
						ImagePath = "D:\\Zdjęcia\\RSI\\interstellar.jpg",
						Director = "Christopher Nolan",
						Script = "Christopher Nolan, Jonathan Nolan",
						Genre = "Sci-Fi",
						ProductionYear = 2014,
						Duration = new TimeSpan(2, 49, 0),
						Grade = 8.0f,
					},
					new Movie
					{
						Name = "Avatar",
						DescriptionShort = "Jake, sparaliżowany były komandos, zostaje wysłany na planetę Pandora, gdzie zaprzyjaźnia się z lokalną społecznością i postanawia jej pomóc.",
						DescriptionLong = "Ludzkość odkrywa planetę o nazwie \"Pandora\". Nowy świat, zamieszkały przez humanoidalną rasę zwaną Na'vi, kryje wiele skarbów i surowców, które dla człowieka mają nieocenioną wartość. Ale tak jak puszka, od której planeta wzięła swoją nazwę, \"Pandora\" niesie ze sobą ryzyko nieszczęścia dla tych, którzy się z nią zetkną. Zbadanie nowej cywilizacji zostaje powierzone byłemu żołnierzowi marines, Jake'owi. Wysłany wbrew swojej woli weteran znajdzie się w trudnej sytuacji, gdy przyjdzie mu się zmierzyć z tajemnicami planety i będzie musiał walczyć o przetrwanie.",
						ImagePath = "D:\\Zdjęcia\\RSI\\avatar.jpg",
						Director = "James Cameron",
						Script = "James Cameron",
						Genre = "Sci-Fi",
						ProductionYear = 2009,
						Duration = new TimeSpan(2, 42, 0),
						Grade = 7.4f,
					},
					new Movie
					{
						Name = "Avengers: Endgame",
						DescriptionShort = "Po wymazaniu połowy życia we Wszechświecie przez Thanosa Avengersi starają się zrobić wszystko, co konieczne, aby pokonać szalonego tytana.",
						DescriptionLong = "Po dramatycznych wydarzeniach z filmu \"Avengers: Wojna bez granic\" Iron Man samotnie dryfuje po kosmosie, połowa superbohaterów przestała istnieć, a Kapitan Ameryka próbuje pogodzić się z porażką. Świat już nigdy nie będzie taki sam, a ocaleni muszą nauczyć się żyć w nowych warunkach. Nie wszyscy jednak dają za wygraną. Gdy w bazie Avengers pojawia się Ant-Man, w głowach herosów rodzi się szalony plan zmiany obecnego stanu rzeczy. Czy jest możliwe odwrócenie \"Efektu Thanosa\" i przywrócenie do naszego świata tych, co odeszli? Iron Man, Kapitan Ameryka, Thor, Hulk, Czarna Wdowa i pozostali zbierają się ponownie, żeby wyruszyć w pełną przygód podróż po najważniejszych momentach w historii Avengers.",
						ImagePath = "D:\\Zdjęcia\\RSI\\avengers_endgame.jpg",
						Director = "Anthony Russo, Joe Russo",
						Script = "Christopher Markus, Stephen McFeely",
						Genre = "Akcja, Sci-Fi",
						ProductionYear = 2019,
						Duration = new TimeSpan(3, 2, 0),
						Grade = 8.0f,
					},
					new Movie
					{
						Name = "Challengers",
						DescriptionShort = "Tashi, była gwiazda tenisa, zostaje trenerką swojego męża, który aby przerwać pasmo porażek musi zmierzyć się na korcie ze swoim byłym najlepszym przyjacielem, a przy tym byłym partnerem żony.",
						DescriptionLong = "Wizjonerski twórca Luca Guadagnino przedstawia \"Challengers\" z Zendayą w roli Tashi Duncan, która jako złote dziecko tenisa została potem trenerką, bezkompromisową na korcie i poza nim. Jej mąż (Mike Faist - \"West Side Story\") jest mistrzem, który przeżywa zawodowy kryzys. Tashi postanawia przywrócić mu dawną chwałę, ale oboje czeka niespodzianka, kiedy przeciwnikiem okazuje się inny przebrzmiały czempion Patrick (Josh O'Connor - \"The Crown\") - jego były najlepszy przyjaciel i dawny chłopak Tashi. Kiedy przeszłość zderza się z teraźniejszością, a napięcie sięga zenitu, Tashi musi zapytać samą siebie, jaka będzie cena wygranej.",
						ImagePath = "D:\\Zdjęcia\\RSI\\challengers.jpg",
						Director = "Luca Guadagnino",
						Script = "Justin Kuritzkes",
						Genre = "Dramat, Sportowy",
						ProductionYear = 2024,
						Duration = new TimeSpan(2, 11, 0),
						Grade = 7.2f,
					},
					new Movie
					{
						Name = "Królestwo planety małp",
						DescriptionShort = "W świecie zdominowanym przez żyjące w harmonii małpy ludzie zostali zepchnięci na margines. Gdy nowy przywódca małp buduje swoje imperium, młody samiec wyrusza w niezwykle trudną podróż, która zmieni jego dotychczasowe wyobrażenie o przeszłości, a nowe decyzje wpłyną na przyszłość zarówno małp, jak i ludzi.",
						DescriptionLong = "",
						ImagePath = "D:\\Zdjęcia\\RSI\\krolestwo_planety_malp.jpg",
						Director = "Wes Ball",
						Script = "Josh Friedman",
						Genre = "Akcja, Sci-Fi",
						ProductionYear = 2024,
						Duration = new TimeSpan(2, 25, 0),
						Grade = 7.2f,
					},
				});;

				context.SaveChanges();
			}


			if (!context.Playings.Any())
			{
				context.Playings.AddRange(new List<Playing>()
				{
					new Playing
					{
						HallId = 1,
						Hall = GetById<CinemaHall>(1),
						MovieId = 1,
						Movie = GetById<Movie>(1),
						Date = DateTime.Now.AddDays(365),
						State = PlayingState.ComingSoon,
						TicketCost = 30
					},
					new Playing
					{
						HallId = 1,
						Hall = GetById<CinemaHall>(1),
						MovieId = 2,
						Movie = GetById<Movie>(2),
						Date = DateTime.Now.AddDays(366),
						State = PlayingState.ComingSoon,
						TicketCost = 38
					},
					new Playing
					{
						HallId = 3,
						Hall = GetById<CinemaHall>(3),
						MovieId = 5,
						Movie = GetById<Movie>(5),
						Date = DateTime.Now.AddDays(369),
						State = PlayingState.ComingSoon,
						TicketCost = 70,
					},
					new Playing
					{
						HallId = 2,
						Hall = GetById<CinemaHall>(2),
						MovieId = 3,
						Movie = GetById<Movie>(3),
						Date = DateTime.Now.AddDays(367),
						State = PlayingState.ComingSoon,
						TicketCost = 55
					},
					new Playing
					{
						HallId = 3,
						Hall = GetById<CinemaHall>(3),
						MovieId = 1,
						Movie = GetById<Movie>(1),
						Date = DateTime.Now.AddDays(368),
						State = PlayingState.ComingSoon,
						TicketCost = 30
					},
					new Playing
					{
						HallId = 3,
						Hall = GetById<CinemaHall>(3),
						MovieId = 3,
						Movie = GetById<Movie>(3),
						Date = DateTime.Now.AddDays(369),
						State = PlayingState.ComingSoon,
						TicketCost = 70,
					},
					new Playing
					{
						HallId = 3,
						Hall = GetById<CinemaHall>(3),
						MovieId = 4,
						Movie = GetById<Movie>(4),
						Date = DateTime.Now.AddDays(368),
						State = PlayingState.ComingSoon,
						TicketCost = 30
					},
					new Playing
					{
						HallId = 2,
						Hall = GetById<CinemaHall>(3),
						MovieId = 5,
						Movie = GetById<Movie>(5),
						Date = DateTime.Now.AddDays(369),
						State = PlayingState.ComingSoon,
						TicketCost = 70,
					},
					new Playing
					{
						HallId = 2,
						Hall = GetById<CinemaHall>(2),
						MovieId = 2,
						Movie = GetById<Movie>(2),
						Date = DateTime.Now.AddDays(366),
						State = PlayingState.ComingSoon,
						TicketCost = 35,
					},
				});

				context.SaveChanges();
			}
		}

		private T GetById<T>(int id) where T : class, new()
		{
			if (typeof(T) == typeof(CinemaHall))
			{
				return context.CinemaHalls.FirstOrDefault(x => x.Id == id) as T;
			}

			else if (typeof(T) == typeof(Movie))
			{
				return context.Movies.FirstOrDefault(x => x.Id == id) as T;
			}

			else if (typeof(T) == typeof(Playing))
			{
				return context.Playings.FirstOrDefault(x => x.Id == id) as T;
			}

			else if (typeof(T) == typeof(User))
			{
				return context.Users.FirstOrDefault(x => x.Id == id) as T;
			}

			else if (typeof(T) == typeof(Reservation))
			{
				return context.Reservations.FirstOrDefault(x => x.Id == id) as T;
			}

			else if (typeof(T) == typeof(Cinema))
			{
				return context.Cinemas.FirstOrDefault(x => x.Id == id) as T;
			}

			return null;
		}
	}
}
