using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class fixmissingseedConfigSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            // Idempotent: chỉ insert ConfigSystems mặc định nếu chưa có row Id=1
            // (DB có thể đã có sẵn row cấu hình -> tránh trùng khóa chính)
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [ConfigSystems] WHERE [Id] = 1)
BEGIN
    SET IDENTITY_INSERT [ConfigSystems] ON;
    INSERT INTO [ConfigSystems] ([Id], [BankAccountName], [BankName], [BankNumber], [DiscordUserIds], [LinkFBAdmin], [LinkYoutube], [TelegramUserIds])
    VALUES (1, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
    SET IDENTITY_INSERT [ConfigSystems] OFF;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConfigSystems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "81ee82fe-56f4-404a-a06c-2c7c6849c9cc");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "d32b2cc9-aabe-43b2-8bc0-66c8dfd77080",
                column: "ConcurrencyStamp",
                value: "537cf5f8-9b32-414f-b64b-da9ac05139d1");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "90a70351-e535-43b4-bca2-842ab6d32b83", "AQAAAAEAACcQAAAAEF3HqXkOyhcvpnfVB5zDnwZFZ/yBAGo4x71iqNz3XPNUM/I+dqIMeUiAGMl2jpytWA==" });
        }
    }
}
