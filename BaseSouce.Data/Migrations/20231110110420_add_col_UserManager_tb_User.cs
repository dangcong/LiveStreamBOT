using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_UserManager_tb_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserManager",
                table: "ManagerBOTs",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserManager",
                table: "AppUsers",
                type: "nvarchar(128)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_ManagerBOTs_UserManager",
                table: "ManagerBOTs",
                column: "UserManager");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_UserManager",
                table: "AppUsers",
                column: "UserManager");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_AppUsers_UserManager",
                table: "AppUsers",
                column: "UserManager",
                principalTable: "AppUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerBOTs_AppUsers_UserManager",
                table: "ManagerBOTs",
                column: "UserManager",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_AppUsers_UserManager",
                table: "AppUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagerBOTs_AppUsers_UserManager",
                table: "ManagerBOTs");

            migrationBuilder.DropIndex(
                name: "IX_ManagerBOTs_UserManager",
                table: "ManagerBOTs");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_UserManager",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "UserManager",
                table: "ManagerBOTs");

            migrationBuilder.DropColumn(
                name: "UserManager",
                table: "AppUsers");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "de6e3578-4f6a-478e-a649-b4341395affd");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bdff738a-f320-4925-a2e8-faf72c8a2f93", "AQAAAAEAACcQAAAAEGgFyGg5ZgQ17h6KNA0E43VqjNNhEP01T5aTSFH7ncTN//jB1fYUV9X9K3Rbrr3ZWQ==" });
        }
    }
}
