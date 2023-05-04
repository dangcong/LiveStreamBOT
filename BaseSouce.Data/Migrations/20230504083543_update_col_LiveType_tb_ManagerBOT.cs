using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class update_col_LiveType_tb_ManagerBOT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "LiveType",
                table: "ManagerBOTs",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "fdb66ee7-0e54-4a79-805a-ef480bdd8e7e");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ad8402a0-0b7b-49c9-b5b9-63d823891c11", "AQAAAAEAACcQAAAAEE0BzDAkuCOXymVXsZxng19/+sykFsZTrlLDRjLdho1U4uHlPP6TA7nqYFxURYoWgg==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "LiveType",
                table: "ManagerBOTs",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

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
    }
}
