using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAdopt.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdRelatiiTabele : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Comandas_idAnimal",
                table: "Comandas",
                column: "idAnimal");

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_idClient",
                table: "Comandas",
                column: "idClient");

            migrationBuilder.AddForeignKey(
                name: "FK_Comandas_Animals_idAnimal",
                table: "Comandas",
                column: "idAnimal",
                principalTable: "Animals",
                principalColumn: "idAnimal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comandas_Clients_idClient",
                table: "Comandas",
                column: "idClient",
                principalTable: "Clients",
                principalColumn: "idClient",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comandas_Animals_idAnimal",
                table: "Comandas");

            migrationBuilder.DropForeignKey(
                name: "FK_Comandas_Clients_idClient",
                table: "Comandas");

            migrationBuilder.DropIndex(
                name: "IX_Comandas_idAnimal",
                table: "Comandas");

            migrationBuilder.DropIndex(
                name: "IX_Comandas_idClient",
                table: "Comandas");
        }
    }
}
