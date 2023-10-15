using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_DiskTotal_DiskUsed_tb_ManagementBot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DiskTotal",
                table: "ManagerBOTs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "DiskUsed",
                table: "ManagerBOTs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiskTotal",
                table: "ManagerBOTs");

            migrationBuilder.DropColumn(
                name: "DiskUsed",
                table: "ManagerBOTs");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "7af1165f-1f8f-4b60-bee5-bb3a7d154457");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e98811bb-9b29-4917-945c-5fdc43321de5", "AQAAAAEAACcQAAAAEL6ZYmBFmusqiRa/CXoLrTQ8hOq3MOV1HuFSRe3XMjcf1K9jdlMT5zLuTRZvs+Zweg==" });
        }
    }
}
