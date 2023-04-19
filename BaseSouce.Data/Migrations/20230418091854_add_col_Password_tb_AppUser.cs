using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_Password_tb_AppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "AppUsers");

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
    }
}
