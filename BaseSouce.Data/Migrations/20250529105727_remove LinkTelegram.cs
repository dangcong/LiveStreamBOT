using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class removeLinkTelegram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkTelegram",
                table: "AppUsers");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "8917027d-3fdd-41b1-abe0-bc55cddfd741");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "d32b2cc9-aabe-43b2-8bc0-66c8dfd77080",
                column: "ConcurrencyStamp",
                value: "fffb431f-85fe-4a2c-94ae-b62d194f76bb");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e22cc615-9fb7-46fe-8998-4bc14c62a969", "AQAAAAEAACcQAAAAELRu9QUSGfsCuBqslHOFKPSr4RyNZ7rnEHEyd4MadvHqKoZcjefJ2lvJ7cbi9NY6lw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkTelegram",
                table: "AppUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "42c3c03f-ef08-4c1d-93f3-ba51ad6175cc");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "d32b2cc9-aabe-43b2-8bc0-66c8dfd77080",
                column: "ConcurrencyStamp",
                value: "cfc30bea-f347-4ab9-9a18-af819a735ace");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e80c35cc-6482-4f01-a8a2-1577783c4c85", "AQAAAAEAACcQAAAAEF/GB3Kcf5d0ES7vDF9fVFt66IJL98QMjnVE/52BOYhqPG+xVbJ0u+vRP8EAsofwXw==" });
        }
    }
}
