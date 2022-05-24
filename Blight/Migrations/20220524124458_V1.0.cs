using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Blight.Migrations
{
    public partial class V10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBullyTreshold = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Banned = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Master" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Admin" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "User" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Banned", "DateOfBirth", "Email", "FirstName", "LastName", "Nationality", "Password", "RoleId" },
                values: new object[] { 1, false, new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "master@example.com", "Master", "Master", "Poland", "AQAAAAEAACcQAAAAEIqVkVbKzIPnKViW//zCSfdAZkiGiU3e5sJ1ewbtsF966WFkhXOvNbHIFMYFgKzHpQ==", 1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Banned", "DateOfBirth", "Email", "FirstName", "LastName", "Nationality", "Password", "RoleId" },
                values: new object[] { 2, false, new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@example.com", "Admin", "Admin", "Poland", "AQAAAAEAACcQAAAAEN0vmYl6vhX0785CoKJcTWNVYm3qbIzERBcoGN9MOKp0BdWResQGTaVteQVNxHYhhA==", 2 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Banned", "DateOfBirth", "Email", "FirstName", "LastName", "Nationality", "Password", "RoleId" },
                values: new object[] { 3, false, new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user@example.com", "User", "User", "Poland", "AQAAAAEAACcQAAAAEPX5W8qOzGNKA6fs58nDb2H/uTfY60LiQUGTUMf7Ixqd16olYt9XzXgvekjlh7RGZg==", 3 });

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumberUser_UsersId",
                table: "PhoneNumberUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhoneNumberUser");

            migrationBuilder.DropTable(
                name: "PhoneNumbers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
