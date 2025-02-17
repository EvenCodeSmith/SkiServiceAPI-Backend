using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkiServiceAPI.Migrations
{
    /// <inheritdoc />
    public partial class Passwords2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2b$10$eIm3ck7feyg/tB4LBUSyaOQ198PPKs/cETcg4DdOE8aWsp4QVuls6");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2b$10$NoC03rw5UcCCOhG2peeHdOywOgiJ7SGhtxshVw88ME5A5dg0FiQxa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
