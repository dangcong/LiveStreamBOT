using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_config_tb_StreamHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamHistory_AppUsers_AppUserId",
                table: "StreamHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamHistory_ManagerBOTs_ManagerBOTId",
                table: "StreamHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamHistory",
                table: "StreamHistory");

            migrationBuilder.DropIndex(
                name: "IX_StreamHistory_AppUserId",
                table: "StreamHistory");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "StreamHistory");

            migrationBuilder.RenameTable(
                name: "StreamHistory",
                newName: "StreamHistorys");

            migrationBuilder.RenameIndex(
                name: "IX_StreamHistory_ManagerBOTId",
                table: "StreamHistorys",
                newName: "IX_StreamHistorys_ManagerBOTId");

            migrationBuilder.AlterColumn<string>(
                name: "Video",
                table: "StreamHistorys",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StreamHistorys",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StreamKey",
                table: "StreamHistorys",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RTMP",
                table: "StreamHistorys",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "StreamHistorys",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BotId",
                table: "StreamHistorys",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Audio",
                table: "StreamHistorys",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamHistorys",
                table: "StreamHistorys",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "0a8c0216-eaa2-4dfe-91e1-35f75dbe9e1c");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bc6fd493-b48b-44c2-858a-5744f0e6dbf8", "AQAAAAEAACcQAAAAEKXeCo8/C8TBdKpDHsL0ThogXC5pOJ9Ukabw68jsEiRBmDiyv3re0OEYWbRXnMHSGQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_StreamHistorys_UserId",
                table: "StreamHistorys",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamHistorys_AppUsers_UserId",
                table: "StreamHistorys",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamHistorys_ManagerBOTs_ManagerBOTId",
                table: "StreamHistorys",
                column: "ManagerBOTId",
                principalTable: "ManagerBOTs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamHistorys_AppUsers_UserId",
                table: "StreamHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamHistorys_ManagerBOTs_ManagerBOTId",
                table: "StreamHistorys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamHistorys",
                table: "StreamHistorys");

            migrationBuilder.DropIndex(
                name: "IX_StreamHistorys_UserId",
                table: "StreamHistorys");

            migrationBuilder.RenameTable(
                name: "StreamHistorys",
                newName: "StreamHistory");

            migrationBuilder.RenameIndex(
                name: "IX_StreamHistorys_ManagerBOTId",
                table: "StreamHistory",
                newName: "IX_StreamHistory_ManagerBOTId");

            migrationBuilder.AlterColumn<string>(
                name: "Video",
                table: "StreamHistory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StreamHistory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StreamKey",
                table: "StreamHistory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RTMP",
                table: "StreamHistory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "StreamHistory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BotId",
                table: "StreamHistory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Audio",
                table: "StreamHistory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "StreamHistory",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamHistory",
                table: "StreamHistory",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "c80fc95b-2aeb-4c44-8c0a-653e52d54bbd");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "16e156c8-c04a-4409-a705-10a5841608a7", "AQAAAAEAACcQAAAAEImpbNeJz5f2DaBmgIiCS4YQqJrHi/ZK9XkF7H7AbYLlbD58uAx19vGSpFZISZDmNA==" });

            migrationBuilder.CreateIndex(
                name: "IX_StreamHistory_AppUserId",
                table: "StreamHistory",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamHistory_AppUsers_AppUserId",
                table: "StreamHistory",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamHistory_ManagerBOTs_ManagerBOTId",
                table: "StreamHistory",
                column: "ManagerBOTId",
                principalTable: "ManagerBOTs",
                principalColumn: "Id");
        }
    }
}
