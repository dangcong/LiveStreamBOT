using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_tb_StreamHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfThreadsCreate",
                table: "ServicePacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredTime",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NumberOfThreads",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfThreads1080",
                table: "AppUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfThreads1080Create",
                table: "AppUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfThreads4K",
                table: "AppUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfThreads4KCreate",
                table: "AppUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StreamHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerBOTId = table.Column<int>(type: "int", nullable: false),
                    BotId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RTMP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LiveType = table.Column<byte>(type: "tinyint", nullable: false),
                    Video = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Audio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LiveOrder = table.Column<byte>(type: "tinyint", nullable: false),
                    IsForever = table.Column<bool>(type: "bit", nullable: false),
                    IsLiveNow = table.Column<bool>(type: "bit", nullable: false),
                    StartTimeLive = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTimeLive = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updatedtime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(128)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamHistory_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StreamHistory_ManagerBOTs_ManagerBOTId",
                        column: x => x.ManagerBOTId,
                        principalTable: "ManagerBOTs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "5e98f0e7-96f7-4215-b160-c52c4255cff4");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4e6e2e33-519e-46e7-9480-c6e917a689b9", "AQAAAAEAACcQAAAAEDhT9QNAA4SqUw+P8fvnx+7MlSlAFQ80RKLS3AiSK8tNe+MaeJDfjpFHqiC/K/hfEQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_StreamHistory_AppUserId",
                table: "StreamHistory",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamHistory_ManagerBOTId",
                table: "StreamHistory",
                column: "ManagerBOTId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamHistory");

            migrationBuilder.DropColumn(
                name: "NumberOfThreadsCreate",
                table: "ServicePacks");

            migrationBuilder.DropColumn(
                name: "ExpiredTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "NumberOfThreads",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "NumberOfThreads1080",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "NumberOfThreads1080Create",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "NumberOfThreads4K",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "NumberOfThreads4KCreate",
                table: "AppUsers");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "9655dd63-d569-4b7b-8425-8c40285d44c8");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "11daf4e8-2594-414b-ac09-bc6416deb9e0", "AQAAAAEAACcQAAAAEHSB4t1ERs7C/X4TdxMW6exDQQInKcIOEesZzhKYlTmZJ6ecn8rfTc9Il/lSBJAVVA==" });
        }
    }
}
