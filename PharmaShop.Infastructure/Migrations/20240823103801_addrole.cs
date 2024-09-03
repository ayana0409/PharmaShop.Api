using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaShop.Application.Migrations
{
    /// <inheritdoc />
    public partial class addrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ApplicationRole",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRole_ApplicationUserId",
                table: "ApplicationRole",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationRole_ApplicationUser_ApplicationUserId",
                table: "ApplicationRole",
                column: "ApplicationUserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationRole_ApplicationUser_ApplicationUserId",
                table: "ApplicationRole");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationRole_ApplicationUserId",
                table: "ApplicationRole");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ApplicationRole");
        }
    }
}
