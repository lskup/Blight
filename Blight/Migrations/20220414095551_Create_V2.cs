using Microsoft.EntityFrameworkCore.Migrations;

namespace Blight.Migrations
{
    public partial class Create_V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "PhoneNumberUser",
                columns: table => new
                {
                    BlockedNumbersId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumberUser", x => new { x.BlockedNumbersId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_PhoneNumberUser_PhoneNumbers_BlockedNumbersId",
                        column: x => x.BlockedNumbersId,
                        principalTable: "PhoneNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneNumberUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumberUser_UsersId",
                table: "PhoneNumberUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhoneNumberUser");

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
    }
}
