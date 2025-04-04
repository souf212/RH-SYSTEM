using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFA_TEMPLATE.Migrations
{
    /// <inheritdoc />
    public partial class AddJustificatifPathToConges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JustificatifPath",
                table: "Conges",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JustificatifPath",
                table: "Conges");
        }
    }
}
