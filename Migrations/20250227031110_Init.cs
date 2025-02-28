using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prenom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Adresse = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CIN = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Login = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "VARCHAR(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telephone = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Administrateurs",
                columns: table => new
                {
                    IdAdmin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrateurs", x => x.IdAdmin);
                    table.ForeignKey(
                        name: "FK_Administrateurs_Utilisateurs_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employes",
                columns: table => new
                {
                    IdEmploye = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employes", x => x.IdEmploye);
                    table.ForeignKey(
                        name: "FK_Employes_Utilisateurs_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Absences",
                columns: table => new
                {
                    IdAbsences = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateAbsence = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Justification = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdEmploye = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Absences", x => x.IdAbsences);
                    table.ForeignKey(
                        name: "FK_Absences_Employes_IdEmploye",
                        column: x => x.IdEmploye,
                        principalTable: "Employes",
                        principalColumn: "IdEmploye",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Conges",
                columns: table => new
                {
                    IdConges = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Motif = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateDebut = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IdEmploye = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdminComment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conges", x => x.IdConges);
                    table.ForeignKey(
                        name: "FK_Conges_Employes_IdEmploye",
                        column: x => x.IdEmploye,
                        principalTable: "Employes",
                        principalColumn: "IdEmploye",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Contrats",
                columns: table => new
                {
                    IdContrat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SalaireDeBase = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TypeContrat = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdEmploye = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contrats", x => x.IdContrat);
                    table.ForeignKey(
                        name: "FK_Contrats_Employes_IdEmploye",
                        column: x => x.IdEmploye,
                        principalTable: "Employes",
                        principalColumn: "IdEmploye",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Plannings",
                columns: table => new
                {
                    IdPlanning = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateDebut = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Statut = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdEmploye = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plannings", x => x.IdPlanning);
                    table.ForeignKey(
                        name: "FK_Plannings_Employes_IdEmploye",
                        column: x => x.IdEmploye,
                        principalTable: "Employes",
                        principalColumn: "IdEmploye",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pointages",
                columns: table => new
                {
                    IdPointage = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HeureEntree = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    HeureSortie = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IdEmploye = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pointages", x => x.IdPointage);
                    table.ForeignKey(
                        name: "FK_Pointages_Employes_IdEmploye",
                        column: x => x.IdEmploye,
                        principalTable: "Employes",
                        principalColumn: "IdEmploye",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReconnaissanceFaciales",
                columns: table => new
                {
                    IdReconnaissanceFaciale = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ImageFaciale = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HeureDetectee = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IdEmploye = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconnaissanceFaciales", x => x.IdReconnaissanceFaciale);
                    table.ForeignKey(
                        name: "FK_ReconnaissanceFaciales_Employes_IdEmploye",
                        column: x => x.IdEmploye,
                        principalTable: "Employes",
                        principalColumn: "IdEmploye",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Salaires",
                columns: table => new
                {
                    IdSalaire = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Grade = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Competence = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MontantBrut = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MontantNet = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    IdEmploye = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaires", x => x.IdSalaire);
                    table.ForeignKey(
                        name: "FK_Salaires_Employes_IdEmploye",
                        column: x => x.IdEmploye,
                        principalTable: "Employes",
                        principalColumn: "IdEmploye",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Taches",
                columns: table => new
                {
                    IdTaches = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Titre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Statut = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Deadline = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IdEmploye = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taches", x => x.IdTaches);
                    table.ForeignKey(
                        name: "FK_Taches_Employes_IdEmploye",
                        column: x => x.IdEmploye,
                        principalTable: "Employes",
                        principalColumn: "IdEmploye",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HistoriqueAbsences",
                columns: table => new
                {
                    IdHistoriqueAbsences = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdAbsences = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueAbsences", x => x.IdHistoriqueAbsences);
                    table.ForeignKey(
                        name: "FK_HistoriqueAbsences_Absences_IdAbsences",
                        column: x => x.IdAbsences,
                        principalTable: "Absences",
                        principalColumn: "IdAbsences",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HistoriqueConges",
                columns: table => new
                {
                    IdHistoriqueConges = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdConges = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueConges", x => x.IdHistoriqueConges);
                    table.ForeignKey(
                        name: "FK_HistoriqueConges_Conges_IdConges",
                        column: x => x.IdConges,
                        principalTable: "Conges",
                        principalColumn: "IdConges",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FichesDePaie",
                columns: table => new
                {
                    IdFicheDePaie = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SalaireNet = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    IdContrat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FichesDePaie", x => x.IdFicheDePaie);
                    table.ForeignKey(
                        name: "FK_FichesDePaie_Contrats_IdContrat",
                        column: x => x.IdContrat,
                        principalTable: "Contrats",
                        principalColumn: "IdContrat",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_IdEmploye",
                table: "Absences",
                column: "IdEmploye");

            migrationBuilder.CreateIndex(
                name: "IX_Administrateurs_IdUtilisateur",
                table: "Administrateurs",
                column: "IdUtilisateur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conges_IdEmploye",
                table: "Conges",
                column: "IdEmploye");

            migrationBuilder.CreateIndex(
                name: "IX_Contrats_IdEmploye",
                table: "Contrats",
                column: "IdEmploye");

            migrationBuilder.CreateIndex(
                name: "IX_Employes_IdUtilisateur",
                table: "Employes",
                column: "IdUtilisateur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FichesDePaie_IdContrat",
                table: "FichesDePaie",
                column: "IdContrat");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueAbsences_IdAbsences",
                table: "HistoriqueAbsences",
                column: "IdAbsences");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueConges_IdConges",
                table: "HistoriqueConges",
                column: "IdConges");

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_IdEmploye",
                table: "Plannings",
                column: "IdEmploye");

            migrationBuilder.CreateIndex(
                name: "IX_Pointages_IdEmploye",
                table: "Pointages",
                column: "IdEmploye");

            migrationBuilder.CreateIndex(
                name: "IX_ReconnaissanceFaciales_IdEmploye",
                table: "ReconnaissanceFaciales",
                column: "IdEmploye");

            migrationBuilder.CreateIndex(
                name: "IX_Salaires_IdEmploye",
                table: "Salaires",
                column: "IdEmploye");

            migrationBuilder.CreateIndex(
                name: "IX_Taches_IdEmploye",
                table: "Taches",
                column: "IdEmploye");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrateurs");

            migrationBuilder.DropTable(
                name: "FichesDePaie");

            migrationBuilder.DropTable(
                name: "HistoriqueAbsences");

            migrationBuilder.DropTable(
                name: "HistoriqueConges");

            migrationBuilder.DropTable(
                name: "Plannings");

            migrationBuilder.DropTable(
                name: "Pointages");

            migrationBuilder.DropTable(
                name: "ReconnaissanceFaciales");

            migrationBuilder.DropTable(
                name: "Salaires");

            migrationBuilder.DropTable(
                name: "Taches");

            migrationBuilder.DropTable(
                name: "Contrats");

            migrationBuilder.DropTable(
                name: "Absences");

            migrationBuilder.DropTable(
                name: "Conges");

            migrationBuilder.DropTable(
                name: "Employes");

            migrationBuilder.DropTable(
                name: "Utilisateurs");
        }
    }
}
