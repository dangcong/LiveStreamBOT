using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_LinkYoutube_tb_ConfigSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkYoutube",
                table: "ConfigSystems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "9ade18ab-3603-4924-9454-b37c5f855f7f");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "8e1508df-ea04-4102-ac4e-e4d663aa8ea5", "AQAAAAEAACcQAAAAELVzAeWmR7l7O9d1uoCzvMvVGxjTflK0npI9mdp6FIkGFt13PIaUZ7fko02D9ZGODA==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkYoutube",
                table: "ConfigSystems");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "0a403e1f-a5f2-4cf5-85f4-fda7b20c3b5e");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "cdc36ce1-d11a-4531-83c2-773e8ad9af86", "AQAAAAEAACcQAAAAEE5Yg7R8d3huVVt26gl2hNsvbHNiGWEkBQSoQj9oBB3n7lNEpAh//YgIyYpHBzgPCg==" });
        }
    }
}
