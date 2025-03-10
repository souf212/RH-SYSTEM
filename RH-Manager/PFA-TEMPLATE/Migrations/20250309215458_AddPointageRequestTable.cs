using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFA_TEMPLATE.Migrations
{
    /// <inheritdoc />
    public partial class AddPointageRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "HeureSortie",
                table: "Pointages",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<int>(
                name: "EmployesIdEmploye",
                table: "Pointages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pointages_EmployesIdEmploye",
                table: "Pointages",
                column: "EmployesIdEmploye");

            migrationBuilder.AddForeignKey(
                name: "FK_Pointages_Employes_EmployesIdEmploye",
                table: "Pointages",
                column: "EmployesIdEmploye",
                principalTable: "Employes",
                principalColumn: "IdEmploye");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pointages_Employes_EmployesIdEmploye",
                table: "Pointages");

            migrationBuilder.DropIndex(
                name: "IX_Pointages_EmployesIdEmploye",
                table: "Pointages");

            migrationBuilder.DropColumn(
                name: "EmployesIdEmploye",
                table: "Pointages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HeureSortie",
                table: "Pointages",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
