using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_tb_UserManagerBOT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserManagerBOTs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    ManagerBOTId = table.Column<int>(type: "int", nullable: false),
                    LiveType = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserManagerBOTs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserManagerBOTs_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserManagerBOTs_ManagerBOTs_ManagerBOTId",
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
                value: "4fbe5a3e-b58a-44ee-9e34-8b05040b61f0");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3cd49e7d-0749-459d-82a9-e1059cf02981", "AQAAAAEAACcQAAAAEAeIw/JUCQftGx4g/D/t/WMbjOPCyH6BY9M4hNpM+2dLFJLV/suFCSEjVWkOaxn+PA==" });

            migrationBuilder.CreateIndex(
                name: "IX_UserManagerBOTs_ManagerBOTId",
                table: "UserManagerBOTs",
                column: "ManagerBOTId");

            migrationBuilder.CreateIndex(
                name: "IX_UserManagerBOTs_UserId",
                table: "UserManagerBOTs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserManagerBOTs");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "86d62c32-e1eb-4d19-b42b-b8fcdba2870b");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ccd146ee-fd2e-4883-8cdc-d5a3b3a7834a", "AQAAAAEAACcQAAAAEJlTjhYpzsef5K5s5DYJB8Y9vNbLATavFQqTOik3ZfM3m2rELKgVtXMPGK0PGmTTZg==" });
        }
    }
}
