using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    public partial class add_col_CPU_RAM_Bandwidth_tb_ManagerBOT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamHistory_ManagerBOTs_ManagerBOTId",
                table: "StreamHistory");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerBOTId",
                table: "StreamHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "Bandwidth",
                table: "ManagerBOTs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CPU",
                table: "ManagerBOTs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfThreads",
                table: "ManagerBOTs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "RAM",
                table: "ManagerBOTs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "727a060b-3873-47b5-b372-ea69ed3c7c6a");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ab0ed0a3-86b8-4836-9828-033822fe4360", "AQAAAAEAACcQAAAAEGAnjYnoVRjDEVItBD1wFrz2DGeC10e2GQal8CXPNw067gHQ+C1d8yMSuPtmSW9VGg==" });

            migrationBuilder.AddForeignKey(
                name: "FK_StreamHistory_ManagerBOTs_ManagerBOTId",
                table: "StreamHistory",
                column: "ManagerBOTId",
                principalTable: "ManagerBOTs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamHistory_ManagerBOTs_ManagerBOTId",
                table: "StreamHistory");

            migrationBuilder.DropColumn(
                name: "Bandwidth",
                table: "ManagerBOTs");

            migrationBuilder.DropColumn(
                name: "CPU",
                table: "ManagerBOTs");

            migrationBuilder.DropColumn(
                name: "NumberOfThreads",
                table: "ManagerBOTs");

            migrationBuilder.DropColumn(
                name: "RAM",
                table: "ManagerBOTs");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerBOTId",
                table: "StreamHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "c1105ce5-9dbc-49a9-a7d5-c963b6daa62a",
                column: "ConcurrencyStamp",
                value: "5e98f0e7-96f7-4215-b160-c52c4255cff4");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "ffded6b0-3769-4976-841b-69459049a62d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4e6e2e33-519e-46e7-9480-c6e917a689b9", "AQAAAAEAACcQAAAAEDhT9QNAA4SqUw+P8fvnx+7MlSlAFQ80RKLS3AiSK8tNe+MaeJDfjpFHqiC/K/hfEQ==" });

            migrationBuilder.AddForeignKey(
                name: "FK_StreamHistory_ManagerBOTs_ManagerBOTId",
                table: "StreamHistory",
                column: "ManagerBOTId",
                principalTable: "ManagerBOTs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
