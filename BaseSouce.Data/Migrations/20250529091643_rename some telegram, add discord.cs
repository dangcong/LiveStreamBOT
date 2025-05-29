using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class renamesometelegramadddiscord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //ConfigSystems
            migrationBuilder.RenameColumn(
                name: "Telegrams",
                table: "ConfigSystems",
                newName: "TelegramUserIds");

            migrationBuilder.AddColumn<string>(
                name: "DiscordUserIds",
                table: "ConfigSystems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            //AppUsers
            migrationBuilder.RenameColumn(
                name: "TelegramBOT",
                table: "AppUsers",
                newName: "TelegramUserIdReceiveNotificationsFromStaff");

            migrationBuilder.RenameColumn(
                name: "TelegramAPI",
                table: "AppUsers",
                newName: "TelegramUserId");

            migrationBuilder.AddColumn<string>(
                name: "DiscordUserId",
                table: "AppUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscordUserIdReceiveNotificationsFromStaff",
                table: "AppUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            //seed
            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "81ee82fe-56f4-404a-a06c-2c7c6849c9cc");

            migrationBuilder.InsertData(
                table: "AppRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[] { "d32b2cc9-aabe-43b2-8bc0-66c8dfd77080", "537cf5f8-9b32-414f-b64b-da9ac05139d1", "Manager role", "Manager", "Manager" });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "90a70351-e535-43b4-bca2-842ab6d32b83", "AQAAAAEAACcQAAAAEF3HqXkOyhcvpnfVB5zDnwZFZ/yBAGo4x71iqNz3XPNUM/I+dqIMeUiAGMl2jpytWA==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //seed
            migrationBuilder.DeleteData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "d32b2cc9-aabe-43b2-8bc0-66c8dfd77080");


            //ConfigSystems
            migrationBuilder.DropColumn(
                name: "DiscordUserIds",
                table: "ConfigSystems");

            migrationBuilder.RenameColumn(
                name: "TelegramUserIds",
                table: "ConfigSystems",
                newName: "Telegrams");


            //AppUsers
            migrationBuilder.RenameColumn(
                name: "TelegramUserIdReceiveNotificationsFromStaff",
                table: "AppUsers",
                newName: "TelegramBOT");

            migrationBuilder.RenameColumn(
                name: "TelegramUserId",
                table: "AppUsers",
                newName: "TelegramAPI");

            migrationBuilder.DropColumn(
                name: "DiscordUserId",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "DiscordUserIdReceiveNotificationsFromStaff",
                table: "AppUsers");



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
    }
}
