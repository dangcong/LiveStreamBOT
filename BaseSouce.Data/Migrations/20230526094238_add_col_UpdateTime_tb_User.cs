using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_UpdateTime_tb_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfThreads1080",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "NumberOfThreads4K",
                table: "AppUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdate",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "7af1165f-1f8f-4b60-bee5-bb3a7d154457");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e98811bb-9b29-4917-945c-5fdc43321de5", "AQAAAAEAACcQAAAAEL6ZYmBFmusqiRa/CXoLrTQ8hOq3MOV1HuFSRe3XMjcf1K9jdlMT5zLuTRZvs+Zweg==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "UserUpdate",
                table: "AppUsers");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfThreads1080",
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

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "b17c61b8-cc54-4f37-82b7-5e856c2638d7");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3e962069-fc9c-4d23-9238-45f5f1e96662", "AQAAAAEAACcQAAAAEOcpyUGiHmgvIBMTGOpe0XSVqQ8ZivBGL7yjdi5hV4lhEC0acU2+eXMxkYDR5qYdLA==" });
        }
    }
}
