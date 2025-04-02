using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFA_TEMPLATE.Migrations
{
    /// <inheritdoc />
    public partial class AddEtatContratColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdUtilisateur",
                table: "Contrats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "EtatContrat",
                table: "Contrats",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EtatContrat",
                table: "Contrats");

            migrationBuilder.AlterColumn<int>(
                name: "IdUtilisateur",
                table: "Contrats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
