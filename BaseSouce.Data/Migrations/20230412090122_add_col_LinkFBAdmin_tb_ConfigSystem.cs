using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_LinkFBAdmin_tb_ConfigSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkFBAdmin",
                table: "ConfigSystems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkFBAdmin",
                table: "ConfigSystems");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "7e1c481c-58b1-4e0b-b346-372ab9df79a0");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "21bde9c5-f4aa-4dc6-be19-434b1ba634dd", "AQAAAAEAACcQAAAAECMOCSEvMGpEvoFmiu29ATVIO+eH6VTy4e9ZBPVcXD2IjlJ1A1/ncZOKtUh+wHAqxg==" });
        }
    }
}
