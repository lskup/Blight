using Microsoft.EntityFrameworkCore.Migrations;

namespace Blight.Migrations
{
    public partial class V11_UserEntity_RoleAsVirtual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAEAACcQAAAAEEpn8qdQVaWWluzW2IcLf5yjbS+t76M+omphLqtANilB6tD5s9szsFo2Le3Ze+76kA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "AQAAAAEAACcQAAAAEKh8xvsOpx/3Uf+dtxTTegDEUvF12pFGlezL+RrCjmZv3dEhDHX/poA3LOalmCRhEQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "AQAAAAEAACcQAAAAEHiW/mkDi5jIDMCugasMPNTOLzLbVQDqza1zobf5BvOxSYt0LO82JXggD3fhxSjf9A==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAEAACcQAAAAEIqVkVbKzIPnKViW//zCSfdAZkiGiU3e5sJ1ewbtsF966WFkhXOvNbHIFMYFgKzHpQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "AQAAAAEAACcQAAAAEN0vmYl6vhX0785CoKJcTWNVYm3qbIzERBcoGN9MOKp0BdWResQGTaVteQVNxHYhhA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "AQAAAAEAACcQAAAAEPX5W8qOzGNKA6fs58nDb2H/uTfY60LiQUGTUMf7Ixqd16olYt9XzXgvekjlh7RGZg==");
        }
    }
}
