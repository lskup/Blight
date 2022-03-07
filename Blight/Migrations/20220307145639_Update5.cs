using Microsoft.EntityFrameworkCore.Migrations;

namespace Blight.Migrations
{
    public partial class Update5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBully",
                table: "PhoneNumbers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBully",
                table: "PhoneNumbers");
        }
    }
}
