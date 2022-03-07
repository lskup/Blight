using Microsoft.EntityFrameworkCore.Migrations;

namespace Blight.Migrations
{
    public partial class Update4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBully",
                table: "PhoneNumbers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBully",
                table: "PhoneNumbers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
