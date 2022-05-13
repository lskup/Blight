using Microsoft.EntityFrameworkCore.Migrations;

namespace Blight.Migrations
{
    public partial class V13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsBullyTreshold",
                table: "PhoneNumbers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBullyTreshold",
                table: "PhoneNumbers");
        }
    }
}
