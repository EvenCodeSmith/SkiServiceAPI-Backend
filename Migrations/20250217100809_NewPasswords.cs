using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkiServiceAPI.Migrations
{
    /// <inheritdoc />
    public partial class NewPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2b$10$6KHhXy0V7ptsvrSShn1jwOFFPzJC34SEErFE.Gneafyko3Fy58l32");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2b$10$yU3fociY4o8fQFFQGEyoVujTqZAHECKt720ZiDAqub2l3ntzmMWDS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2b$10$I//na1FtDHJtyWkdyZyu9.kEqOsMUC.k0i46tZ8Vbog9zYyt3eg0O");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "password123");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "securepass");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "securepass123!");
        }
    }
}
