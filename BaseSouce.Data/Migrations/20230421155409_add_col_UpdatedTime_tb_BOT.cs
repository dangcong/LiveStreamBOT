using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_UpdatedTime_tb_BOT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "ManagerBOTs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "86d62c32-e1eb-4d19-b42b-b8fcdba2870b");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ccd146ee-fd2e-4883-8cdc-d5a3b3a7834a", "AQAAAAEAACcQAAAAEJlTjhYpzsef5K5s5DYJB8Y9vNbLATavFQqTOik3ZfM3m2rELKgVtXMPGK0PGmTTZg==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "ManagerBOTs");

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
    }
}
