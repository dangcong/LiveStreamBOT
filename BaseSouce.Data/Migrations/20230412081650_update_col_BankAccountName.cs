using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class update_col_BankAccountName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BankAccountNumber",
                table: "ConfigSystems",
                newName: "BankAccountName");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BankAccountName",
                table: "ConfigSystems",
                newName: "BankAccountNumber");

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
    }
}
