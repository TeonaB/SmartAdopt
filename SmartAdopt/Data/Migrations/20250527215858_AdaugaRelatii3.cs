using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAdopt.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdaugaRelatii3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Postares_AspNetUsers_ApplicationUserId1",
                table: "Postares");

            migrationBuilder.DropIndex(
                name: "IX_Postares_ApplicationUserId1",
                table: "Postares");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Postares");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "Postares",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Postares_ApplicationUserId1",
                table: "Postares",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Postares_AspNetUsers_ApplicationUserId1",
                table: "Postares",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
