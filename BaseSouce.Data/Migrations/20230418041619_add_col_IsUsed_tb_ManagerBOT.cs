using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_IsUsed_tb_ManagerBOT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "ManagerBOTs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "64152f56-5524-4f6d-b478-e076cb4a8221");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0dd1eb8d-27c5-4e29-965f-ff4f4a131540", "AQAAAAEAACcQAAAAEBhbTNNmKIX+6Sw2+Hd1QEoDhX8AEvBG3uiqUcvHPloIDvu5KLdXwhEHthTfKe3hlw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "ManagerBOTs");

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
    }
}
