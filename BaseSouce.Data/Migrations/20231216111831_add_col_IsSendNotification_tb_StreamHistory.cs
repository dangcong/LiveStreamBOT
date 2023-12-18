using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_IsSendNotification_tb_StreamHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSendNotification",
                table: "StreamHistorys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "26dbc5d5-9a39-4fe6-a2b7-448a6b13960f");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "70cbca4b-3188-4730-bb62-a4039639c4ac", "AQAAAAEAACcQAAAAEDsWiiLw2TCzTLK2vNQSNM5eiEz2m0iYvz2v7Y9PwhJ7tAdcG5i9K/v4YuHAoZbS+g==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSendNotification",
                table: "StreamHistorys");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "9b10f313-f7bb-4b90-85af-f7781dda2495");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "88850b79-d1df-4f17-81a2-6d4a2e46ac60", "AQAAAAEAACcQAAAAEPAB5SGAJ8S50vL/j9DZW8d/TCzrwYBENYQVVXth65ljSawSsKP5XljbgbQ16hWOUw==" });
        }
    }
}
