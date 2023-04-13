using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_tb_ConfigSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigSystems", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "290b9bb6-f2ba-404e-8f8f-567942cd965b");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "2e4a868a-74a5-405f-a549-d88804a5dba5", "AQAAAAEAACcQAAAAEGR31Q8A8ph+45a0SqMsRHcMs+EbFCIDFsyOKv/0cxKwVWnXe6swHqbwTO71P5J4PA==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigSystems");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "068b18a2-20ea-4c94-81cd-22e7f4559eb8");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "542bd0cb-6d32-402e-882d-d838590c9ecd", "AQAAAAEAACcQAAAAEFHR0FoVF8RFxtmLTJEGAeHgU04zQAyCSe0gv6i92B8WZDsJMzVhnX11sN+4nV1R9g==" });
        }
    }
}
