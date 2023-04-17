using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_ManagerBOT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "StreamHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "PlatformType",
                table: "StreamHistory",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "RenderStatus",
                table: "StreamHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "ManagerBOTs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "a6a5f9ae-019b-43fb-b743-b4231d2ad538");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bda3bd7f-3b76-4327-9b2a-71d4fbd47ba3", "AQAAAAEAACcQAAAAEFc+hrmRbrlRLcaCJUyzxeynQzv3lvo8O8WQC9sWmq5ypX7kV6eQeSFhLbLLDPjgmg==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "StreamHistory");

            migrationBuilder.DropColumn(
                name: "PlatformType",
                table: "StreamHistory");

            migrationBuilder.DropColumn(
                name: "RenderStatus",
                table: "StreamHistory");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "ManagerBOTs");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "727a060b-3873-47b5-b372-ea69ed3c7c6a");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ab0ed0a3-86b8-4836-9828-033822fe4360", "AQAAAAEAACcQAAAAEGAnjYnoVRjDEVItBD1wFrz2DGeC10e2GQal8CXPNw067gHQ+C1d8yMSuPtmSW9VGg==" });
        }
    }
}
