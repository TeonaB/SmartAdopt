using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAdopt.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChestionarEntitati : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comandas_idAnimal",
                table: "Comandas");

            migrationBuilder.DropIndex(
                name: "IX_Comandas_idClient",
                table: "Comandas");

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_idAnimal",
                table: "Comandas",
                column: "idAnimal");

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_idClient",
                table: "Comandas",
                column: "idClient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comandas_idAnimal",
                table: "Comandas");

            migrationBuilder.DropIndex(
                name: "IX_Comandas_idClient",
                table: "Comandas");

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_idAnimal",
                table: "Comandas",
                column: "idAnimal",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_idClient",
                table: "Comandas",
                column: "idClient",
                unique: true);
        }
    }
}
