using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFA_TEMPLATE.Migrations
{
    /// <inheritdoc />
    public partial class AddContratRelationToUtilisateur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contrats_Employes_IdEmploye",
                table: "Contrats");

            migrationBuilder.RenameColumn(
                name: "IdEmploye",
                table: "Contrats",
                newName: "IdUtilisateur");

            migrationBuilder.RenameIndex(
                name: "IX_Contrats_IdEmploye",
                table: "Contrats",
                newName: "IX_Contrats_IdUtilisateur");

            migrationBuilder.AddForeignKey(
                name: "FK_Contrats_Utilisateurs_IdUtilisateur",
                table: "Contrats",
                column: "IdUtilisateur",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contrats_Utilisateurs_IdUtilisateur",
                table: "Contrats");

            migrationBuilder.RenameColumn(
                name: "IdUtilisateur",
                table: "Contrats",
                newName: "IdEmploye");

            migrationBuilder.RenameIndex(
                name: "IX_Contrats_IdUtilisateur",
                table: "Contrats",
                newName: "IX_Contrats_IdEmploye");

            migrationBuilder.AddForeignKey(
                name: "FK_Contrats_Employes_IdEmploye",
                table: "Contrats",
                column: "IdEmploye",
                principalTable: "Employes",
                principalColumn: "IdEmploye",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
