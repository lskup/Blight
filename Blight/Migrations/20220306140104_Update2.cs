using Microsoft.EntityFrameworkCore.Migrations;

namespace Blight.Migrations
{
    public partial class Update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_PhoneNumbers_PhoneNumberId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PhoneNumberId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NumberId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumberId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PhoneNumbers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumbers_UserId",
                table: "PhoneNumbers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneNumbers_Users_UserId",
                table: "PhoneNumbers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhoneNumbers_Users_UserId",
                table: "PhoneNumbers");

            migrationBuilder.DropIndex(
                name: "IX_PhoneNumbers_UserId",
                table: "PhoneNumbers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PhoneNumbers");

            migrationBuilder.AddColumn<int>(
                name: "NumberId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumberId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumberId",
                table: "Users",
                column: "PhoneNumberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PhoneNumbers_PhoneNumberId",
                table: "Users",
                column: "PhoneNumberId",
                principalTable: "PhoneNumbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
