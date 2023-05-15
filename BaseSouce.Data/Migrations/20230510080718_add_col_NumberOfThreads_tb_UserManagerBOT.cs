using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_NumberOfThreads_tb_UserManagerBOT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfThreads",
                table: "UserManagerBOTs",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfThreads",
                table: "UserManagerBOTs");

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
    }
}
