using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_DeletedTime_tb_AppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "LiveType",
                table: "ManagerBOTs",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedTime",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "0be24b15-3a8a-4cd5-b915-a9ba379b71a9");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3681f4b5-da43-4b30-9416-231557eacbf4", "AQAAAAEAACcQAAAAEOmxwtaJ3K4e/cZPM28uVeBpwiaWOjD+g6WQeG0bD/5KIrq+kq8yOyPH76p0YJwsOQ==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LiveType",
                table: "ManagerBOTs");

            migrationBuilder.DropColumn(
                name: "DeletedTime",
                table: "AppUsers");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "4fbe5a3e-b58a-44ee-9e34-8b05040b61f0");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3cd49e7d-0749-459d-82a9-e1059cf02981", "AQAAAAEAACcQAAAAEAeIw/JUCQftGx4g/D/t/WMbjOPCyH6BY9M4hNpM+2dLFJLV/suFCSEjVWkOaxn+PA==" });
        }
    }
}
