using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_tb_ServicePack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "ServicePacks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedTime",
                table: "ServicePacks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "ServicePacks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "ServicePacks",
                type: "datetime2",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "ServicePacks");

            migrationBuilder.DropColumn(
                name: "DeletedTime",
                table: "ServicePacks");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "ServicePacks");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "ServicePacks");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "a69126ff-fda5-4645-82b3-f158e39667d7");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5664704f-9124-45cb-806b-3164f6e10f98", "AQAAAAEAACcQAAAAEIWNffh72RDszAcFeWe2ZT3pM0UOO3k2/T21oUTUnOZSQJnZKQGNo+W4VK/+QCPsCQ==" });
        }
    }
}
