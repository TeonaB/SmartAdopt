using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAdopt.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChestionarEntitati2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RaspChestionaridRasp1",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_RaspChestionaridRasp1",
                table: "Clients",
                column: "RaspChestionaridRasp1");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_RaspChestionars_RaspChestionaridRasp1",
                table: "Clients",
                column: "RaspChestionaridRasp1",
                principalTable: "RaspChestionars",
                principalColumn: "idRasp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_RaspChestionars_RaspChestionaridRasp1",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_RaspChestionaridRasp1",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RaspChestionaridRasp1",
                table: "Clients");
        }
    }
}
