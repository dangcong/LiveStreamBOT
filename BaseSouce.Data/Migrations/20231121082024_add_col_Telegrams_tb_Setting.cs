using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_Telegrams_tb_Setting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BotBackupId",
                table: "StreamHistorys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerBOTBackupId",
                table: "StreamHistorys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeBackup",
                table: "StreamHistorys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBackup",
                table: "ManagerBOTs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Telegrams",
                table: "ManagerBOTs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telegrams",
                table: "ConfigSystems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_StreamHistorys_ManagerBOTBackupId",
                table: "StreamHistorys",
                column: "ManagerBOTBackupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamHistorys_ManagerBOTs_ManagerBOTBackupId",
                table: "StreamHistorys",
                column: "ManagerBOTBackupId",
                principalTable: "ManagerBOTs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamHistorys_ManagerBOTs_ManagerBOTBackupId",
                table: "StreamHistorys");

            migrationBuilder.DropIndex(
                name: "IX_StreamHistorys_ManagerBOTBackupId",
                table: "StreamHistorys");

            migrationBuilder.DropColumn(
                name: "BotBackupId",
                table: "StreamHistorys");

            migrationBuilder.DropColumn(
                name: "ManagerBOTBackupId",
                table: "StreamHistorys");

            migrationBuilder.DropColumn(
                name: "TimeBackup",
                table: "StreamHistorys");

            migrationBuilder.DropColumn(
                name: "IsBackup",
                table: "ManagerBOTs");

            migrationBuilder.DropColumn(
                name: "Telegrams",
                table: "ManagerBOTs");

            migrationBuilder.DropColumn(
                name: "Telegrams",
                table: "ConfigSystems");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "0c2f9968-1d1f-403c-ba19-bfa692bc4c2a");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1859981f-329c-410b-b450-3e9c5d99fa53", "AQAAAAEAACcQAAAAEKBhoxUH87PfWTQkSXOqCBG85wZ6LilsOzK82vOYnvKuXLfmeHEfP9bjJ7eHUWUTvw==" });
        }
    }
}
