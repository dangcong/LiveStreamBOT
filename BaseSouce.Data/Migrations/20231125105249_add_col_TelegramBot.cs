using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_TelegramBot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TelegramBOT",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "3dbb0569-be00-443a-8e86-8643279a3c57");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "cd92b0d3-5221-4b00-9c64-9d31b14eb26e", "AQAAAAEAACcQAAAAELziyzuGqwGvzP18QRjQ74kjhTQpF6QdUwvV6sSBcF2s3HG5O0WoklScHxkELLagsQ==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramBOT",
                table: "AppUsers");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "c6186fc7-eea1-4c2c-b28e-5cc96901f8ad");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bcaf5fc2-ed36-45bc-afa8-4b6b2877a1dc", "AQAAAAEAACcQAAAAEHxU8p35Un9OZhDn8wJ6s20ZoFnVXAtKGcEl/X84OSKEIK6P/FJovg+8LcKBNJwy3A==" });
        }
    }
}
