using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_ManagerBackup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DisconnectedTime",
                table: "StreamHistorys",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLiveBackup",
                table: "StreamHistorys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StreamBackupId",
                table: "StreamHistorys",
                type: "int",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisconnectedTime",
                table: "StreamHistorys");

            migrationBuilder.DropColumn(
                name: "IsLiveBackup",
                table: "StreamHistorys");

            migrationBuilder.DropColumn(
                name: "StreamBackupId",
                table: "StreamHistorys");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "2a3fc94c-c98d-4a35-b434-45bc289f8c36");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "69ffedf6-82b1-4961-b44a-59b514ebff98", "AQAAAAEAACcQAAAAEJMPMsG7OhLX9Yl0DsglN18PS74bc9gnPzKfoINWAlrg2uA/5b0j2a3hQXhTO4MXqg==" });
        }
    }
}
