using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema_Reservations.Migrations
{
    public partial class TicketCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cinemas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinemas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Producer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionYear = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CinemaHalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CinemaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CinemaHalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CinemaHalls_Cinemas_CinemaId",
                        column: x => x.CinemaId,
                        principalTable: "Cinemas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Playings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HallId = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    TicketCost = table.Column<float>(type: "real", nullable: false),
                    CinemaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playings_CinemaHalls_HallId",
                        column: x => x.HallId,
                        principalTable: "CinemaHalls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Playings_Cinemas_CinemaId",
                        column: x => x.CinemaId,
                        principalTable: "Cinemas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Playings_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PlayingId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationCost = table.Column<float>(type: "real", nullable: false),
                    SeatCount = table.Column<int>(type: "int", nullable: false),
                    Seats = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Playings_PlayingId",
                        column: x => x.PlayingId,
                        principalTable: "Playings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CinemaHalls",
                columns: new[] { "Id", "CinemaId", "Number", "Type" },
                values: new object[,]
                {
                    { 1, null, 1, 84 },
                    { 2, null, 2, 168 },
                    { 3, null, 3, 280 }
                });

            migrationBuilder.InsertData(
                table: "Cinemas",
                columns: new[] { "Id", "Address", "Name" },
                values: new object[] { 1, "Wiejska 45A, Białystok", "PBCinema" });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Description", "ImagePath", "Name", "Producer", "ProductionYear" },
                values: new object[,]
                {
                    { 1, "Byt ludzkości na Ziemi dobiega końca wskutek zmian klimatycznych. Grupa naukowców odkrywa tunel czasoprzestrzenny, który umożliwia poszukiwanie nowego domu.", "D:\\Zdjęcia\\RSI\\interstellar.jpg", "Interstellar", "Christopher Nolan", 2014 },
                    { 2, "Jake, sparaliżowany były komandos, zostaje wysłany na planetę Pandora, gdzie zaprzyjaźnia się z lokalną społecznością i postanawia jej pomóc.", "D:\\Zdjęcia\\RSI\\avatar.jpg", "Avatar", "James Cameron", 2009 },
                    { 3, "Po wymazaniu połowy życia we Wszechświecie przez Thanosa Avengersi starają się zrobić wszystko, co konieczne, aby pokonać szalonego tytana.", "D:\\Zdjęcia\\RSI\\avengers_endgame.jpg", "Avengers: Endgame", "Anthony Russo, Joe Russo", 2019 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Password" },
                values: new object[] { 1, "admin@admin.pl", "admin", "admin" });

            migrationBuilder.InsertData(
                table: "Playings",
                columns: new[] { "Id", "CinemaId", "Date", "HallId", "MovieId", "State", "TicketCost" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 5, 13, 20, 7, 56, 427, DateTimeKind.Local).AddTicks(7077), 1, 1, 0, 0f },
                    { 2, null, new DateTime(2025, 5, 14, 20, 7, 56, 427, DateTimeKind.Local).AddTicks(7110), 1, 2, 0, 0f },
                    { 3, null, new DateTime(2025, 5, 14, 20, 7, 56, 427, DateTimeKind.Local).AddTicks(7113), 2, 2, 0, 0f },
                    { 4, null, new DateTime(2025, 5, 15, 20, 7, 56, 427, DateTimeKind.Local).AddTicks(7115), 2, 3, 0, 0f },
                    { 5, null, new DateTime(2025, 5, 16, 20, 7, 56, 427, DateTimeKind.Local).AddTicks(7117), 3, 1, 0, 0f },
                    { 6, null, new DateTime(2025, 5, 17, 20, 7, 56, 427, DateTimeKind.Local).AddTicks(7121), 3, 3, 0, 0f }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CinemaHalls_CinemaId",
                table: "CinemaHalls",
                column: "CinemaId");

            migrationBuilder.CreateIndex(
                name: "IX_Playings_CinemaId",
                table: "Playings",
                column: "CinemaId");

            migrationBuilder.CreateIndex(
                name: "IX_Playings_HallId",
                table: "Playings",
                column: "HallId");

            migrationBuilder.CreateIndex(
                name: "IX_Playings_MovieId",
                table: "Playings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PlayingId",
                table: "Reservations",
                column: "PlayingId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Playings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CinemaHalls");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Cinemas");
        }
    }
}
