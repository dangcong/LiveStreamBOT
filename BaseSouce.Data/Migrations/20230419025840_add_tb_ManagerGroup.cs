using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_tb_ManagerGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagerGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserIdManager = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagerGroups_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ManagerGroups_AppUsers_UserIdManager",
                        column: x => x.UserIdManager,
                        principalTable: "AppUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "0a403e1f-a5f2-4cf5-85f4-fda7b20c3b5e");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "cdc36ce1-d11a-4531-83c2-773e8ad9af86", "AQAAAAEAACcQAAAAEE5Yg7R8d3huVVt26gl2hNsvbHNiGWEkBQSoQj9oBB3n7lNEpAh//YgIyYpHBzgPCg==" });

            migrationBuilder.CreateIndex(
                name: "IX_ManagerGroups_UserId",
                table: "ManagerGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerGroups_UserIdManager",
                table: "ManagerGroups",
                column: "UserIdManager");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerGroups");

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
        }
    }
}
