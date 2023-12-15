using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_DisconnectedTime_tb_ManagerBot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DisconnectedTime",
                table: "ManagerBOTs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSendNotification",
                table: "ManagerBOTs",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisconnectedTime",
                table: "ManagerBOTs");

            migrationBuilder.DropColumn(
                name: "IsSendNotification",
                table: "ManagerBOTs");

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
    }
}
