using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAdopt.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreareBazaProiectModeleBDroluri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nume",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "prenume",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    idAdmin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.idAdmin);
                    table.ForeignKey(
                        name: "FK_Admins_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    idAnimal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    specie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rasa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descriere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    varsta = table.Column<int>(type: "int", nullable: false),
                    nivel_energie = table.Column<int>(type: "int", nullable: false),
                    marime = table.Column<int>(type: "int", nullable: false),
                    nivel_adaptabilitate = table.Column<int>(type: "int", nullable: false),
                    grupa_varsta = table.Column<int>(type: "int", nullable: false),
                    nivel_atentie_necesara = table.Column<int>(type: "int", nullable: false),
                    vaccinuri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tip_alimentatie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    starea_sanatatii = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ultima_verificare_vet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    pret = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.idAnimal);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    idClient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    idRaspChestionar = table.Column<int>(type: "int", nullable: false),
                    CompletedProfile = table.Column<bool>(type: "bit", nullable: false),
                    nr_telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    adresa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.idClient);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Postares",
                columns: table => new
                {
                    idPostare = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    titlu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descriere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_postarii = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postares", x => x.idPostare);
                    table.ForeignKey(
                        name: "FK_Postares_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comandas",
                columns: table => new
                {
                    idComanda = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idClient = table.Column<int>(type: "int", nullable: false),
                    idAnimal = table.Column<int>(type: "int", nullable: false),
                    stare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_comenzii = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total_plata = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    metoda_platii = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    motivatie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientidClient = table.Column<int>(type: "int", nullable: true),
                    AnimalidAnimal = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comandas", x => x.idComanda);
                    table.ForeignKey(
                        name: "FK_Comandas_Animals_AnimalidAnimal",
                        column: x => x.AnimalidAnimal,
                        principalTable: "Animals",
                        principalColumn: "idAnimal");
                    table.ForeignKey(
                        name: "FK_Comandas_Clients_ClientidClient",
                        column: x => x.ClientidClient,
                        principalTable: "Clients",
                        principalColumn: "idClient");
                });

            migrationBuilder.CreateTable(
                name: "RaspChestionars",
                columns: table => new
                {
                    idRasp = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idClient = table.Column<int>(type: "int", nullable: false),
                    ClientidClient = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaspChestionars", x => x.idRasp);
                    table.ForeignKey(
                        name: "FK_RaspChestionars_Clients_ClientidClient",
                        column: x => x.ClientidClient,
                        principalTable: "Clients",
                        principalColumn: "idClient");
                });

            migrationBuilder.CreateTable(
                name: "Comentarius",
                columns: table => new
                {
                    idComentariu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idClient = table.Column<int>(type: "int", nullable: false),
                    idPostare = table.Column<int>(type: "int", nullable: false),
                    descriere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientidClient = table.Column<int>(type: "int", nullable: true),
                    PostareidPostare = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarius", x => x.idComentariu);
                    table.ForeignKey(
                        name: "FK_Comentarius_Clients_ClientidClient",
                        column: x => x.ClientidClient,
                        principalTable: "Clients",
                        principalColumn: "idClient");
                    table.ForeignKey(
                        name: "FK_Comentarius_Postares_PostareidPostare",
                        column: x => x.PostareidPostare,
                        principalTable: "Postares",
                        principalColumn: "idPostare");
                });

            migrationBuilder.CreateTable(
                name: "RaspAnimals",
                columns: table => new
                {
                    idRaspAnimal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idRasp = table.Column<int>(type: "int", nullable: false),
                    idAnimal = table.Column<int>(type: "int", nullable: false),
                    RaspChestionaridRasp = table.Column<int>(type: "int", nullable: true),
                    AnimalidAnimal = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaspAnimals", x => x.idRaspAnimal);
                    table.ForeignKey(
                        name: "FK_RaspAnimals_Animals_AnimalidAnimal",
                        column: x => x.AnimalidAnimal,
                        principalTable: "Animals",
                        principalColumn: "idAnimal");
                    table.ForeignKey(
                        name: "FK_RaspAnimals_RaspChestionars_RaspChestionaridRasp",
                        column: x => x.RaspChestionaridRasp,
                        principalTable: "RaspChestionars",
                        principalColumn: "idRasp");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_ApplicationUserId",
                table: "Admins",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ApplicationUserId",
                table: "Clients",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_AnimalidAnimal",
                table: "Comandas",
                column: "AnimalidAnimal");

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_ClientidClient",
                table: "Comandas",
                column: "ClientidClient");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarius_ClientidClient",
                table: "Comentarius",
                column: "ClientidClient");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarius_PostareidPostare",
                table: "Comentarius",
                column: "PostareidPostare");

            migrationBuilder.CreateIndex(
                name: "IX_Postares_ApplicationUserId",
                table: "Postares",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RaspAnimals_AnimalidAnimal",
                table: "RaspAnimals",
                column: "AnimalidAnimal");

            migrationBuilder.CreateIndex(
                name: "IX_RaspAnimals_RaspChestionaridRasp",
                table: "RaspAnimals",
                column: "RaspChestionaridRasp");

            migrationBuilder.CreateIndex(
                name: "IX_RaspChestionars_ClientidClient",
                table: "RaspChestionars",
                column: "ClientidClient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Comandas");

            migrationBuilder.DropTable(
                name: "Comentarius");

            migrationBuilder.DropTable(
                name: "RaspAnimals");

            migrationBuilder.DropTable(
                name: "Postares");

            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "RaspChestionars");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropColumn(
                name: "nume",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "prenume",
                table: "AspNetUsers");
        }
    }
}
