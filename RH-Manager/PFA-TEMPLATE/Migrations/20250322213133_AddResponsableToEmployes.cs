using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFA_TEMPLATE.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsableToEmployes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResponsableUtilisateurId",
                table: "Employes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employes_ResponsableUtilisateurId",
                table: "Employes",
                column: "ResponsableUtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employes_Utilisateurs_ResponsableUtilisateurId",
                table: "Employes",
                column: "ResponsableUtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id");
        }




        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employes_Utilisateurs_ResponsableUtilisateurId",
                table: "Employes");

            migrationBuilder.DropIndex(
                name: "IX_Employes_ResponsableUtilisateurId",
                table: "Employes");

            migrationBuilder.DropColumn(
                name: "ResponsableUtilisateurId",
                table: "Employes");
        }

    }
}
