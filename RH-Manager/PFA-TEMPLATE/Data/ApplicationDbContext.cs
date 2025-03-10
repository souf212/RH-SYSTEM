using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.viewModels;

namespace PFA_TEMPLATE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for each table
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Employes> Employes { get; set; }
        public DbSet<Administrateur> Administrateurs { get; set; }
        public DbSet<Contrat> Contrats { get; set; }
        public DbSet<Conges> Conges { get; set; }
        public DbSet<Absences> Absences { get; set; }
        public DbSet<HistoriqueAbsences> HistoriqueAbsences { get; set; }
        public DbSet<HistoriqueConges> HistoriqueConges { get; set; }
        public DbSet<ReconnaissanceFaciale> ReconnaissanceFaciales { get; set; }
        public DbSet<Salaire> Salaires { get; set; }
        public DbSet<FicheDePaie> FichesDePaie { get; set; }
        public DbSet<Planning> Plannings { get; set; }
        public DbSet<Pointage> Pointages { get; set; }
        public DbSet<Taches> Taches { get; set; }
        public DbSet<ContraintesPlanning> ContraintesPlanning { get; set; }
        public DbSet<EmploiDuTemps> EmploiDuTemps { get; set; }
        public DbSet<PlageHoraire> PlageHoraire { get; set; }
        public DbSet<CongeBalance> CongeBalances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Utilisateur
            modelBuilder.Entity<Utilisateur>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.CIN)
                .IsUnique();

            // ✅ Employes (One-to-One with Utilisateur)
            modelBuilder.Entity<Employes>()
                .HasKey(e => e.IdEmploye);

            modelBuilder.Entity<Employes>()
                .HasOne(e => e.Utilisateur)
                .WithOne()
                .HasForeignKey<Employes>(e => e.IdUtilisateur)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Conges (One-to-Many with Employe)
            modelBuilder.Entity<Conges>()
                .HasOne(c => c.Employe)
                .WithMany(e => e.Conges)
                .HasForeignKey(c => c.IdEmploye);

            // ✅ CongeBalance (One-to-Many with Employe)
            modelBuilder.Entity<CongeBalance>()
                .HasOne(cb => cb.Employe)
                .WithMany()
                .HasForeignKey(cb => cb.IdEmploye)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CongeBalance>()
                .HasIndex(cb => new { cb.IdEmploye, cb.Annee })
                .IsUnique();

            // ✅ EmploiDuTemps (One-to-Many Planning)
            modelBuilder.Entity<EmploiDuTemps>()
                .HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmploiDuTemps>()
                .HasOne(e => e.ContraintesPlanning)
                .WithMany()
                .HasForeignKey(e => e.ContraintesPlanningId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ PlageHoraire (One-to-Many with EmploiDuTemps)
            modelBuilder.Entity<PlageHoraire>()
                .HasOne(p => p.EmploiDuTemps)
                .WithMany(e => e.PlagesHoraires)
                .HasForeignKey(p => p.EmploiDuTempsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContraintesPlanning>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            // ✅ Ignore computed property in EmploiDuTemps
            modelBuilder.Entity<EmploiDuTemps>()
                .Ignore(e => e.Fitness);

            // ✅ Taches (One-to-Many with Employes)
            modelBuilder.Entity<Taches>()
                .HasKey(t => t.IdTaches);

            modelBuilder.Entity<Taches>()
                .HasOne(t => t.Employe)
                .WithMany(e => e.Taches)
                .HasForeignKey(t => t.IdEmploye)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Administrateur (One-to-One with Utilisateur)
            modelBuilder.Entity<Administrateur>()
                .HasKey(a => a.IdAdmin);

            modelBuilder.Entity<Administrateur>()
                .HasOne(a => a.Utilisateur)
                .WithOne()
                .HasForeignKey<Administrateur>(a => a.IdUtilisateur);

            // ✅ Contrats (One-to-Many with Employes)
            modelBuilder.Entity<Contrat>()
                .HasKey(c => c.IdContrat);

            modelBuilder.Entity<Contrat>()
                .HasOne(c => c.Employe)
                .WithMany(e => e.Contrats)
                .HasForeignKey(c => c.IdEmploye);

            // ✅ Absences (One-to-Many with Employes)
            modelBuilder.Entity<Absences>()
                .HasKey(a => a.IdAbsences);

            modelBuilder.Entity<Absences>()
                .HasOne(a => a.Employe)
                .WithMany(e => e.Absences)
                .HasForeignKey(a => a.IdEmploye);

            // ✅ HistoriqueAbsences (One-to-One with Absences)
            modelBuilder.Entity<HistoriqueAbsences>()
                .HasKey(h => h.IdHistoriqueAbsences);

            modelBuilder.Entity<HistoriqueAbsences>()
                .HasOne(h => h.Absence)
                .WithMany()
                .HasForeignKey(h => h.IdAbsences);

            // ✅ HistoriqueConges (One-to-One with Conges)
            modelBuilder.Entity<HistoriqueConges>()
                .HasKey(h => h.IdHistoriqueConges);

            modelBuilder.Entity<HistoriqueConges>()
                .HasOne(h => h.Conges)
                .WithMany()
                .HasForeignKey(h => h.IdConges);

            // ✅ ReconnaissanceFaciale (One-to-Many with Employes)
            modelBuilder.Entity<ReconnaissanceFaciale>()
                .HasKey(r => r.IdReconnaissanceFaciale);

            modelBuilder.Entity<ReconnaissanceFaciale>()
                .HasOne(r => r.Employe)
                .WithMany(e => e.ReconnaissanceFaciales)
                .HasForeignKey(r => r.IdEmploye);

            // ✅ Salaire (One-to-Many with Employes)
            modelBuilder.Entity<Salaire>()
                .HasKey(s => s.IdSalaire);

            modelBuilder.Entity<Salaire>()
                .HasOne(s => s.Employe)
                .WithMany(e => e.Salaires)
                .HasForeignKey(s => s.IdEmploye);

            // ✅ FicheDePaie (One-to-Many with Contrats)
            modelBuilder.Entity<FicheDePaie>()
                .HasKey(f => f.IdFicheDePaie);

            modelBuilder.Entity<FicheDePaie>()
                .HasOne(f => f.Contrat)
                .WithMany(c => c.FichesDePaie)
                .HasForeignKey(f => f.IdContrat);

            // ✅ Planning (One-to-Many with Employes)
            modelBuilder.Entity<Planning>()
                .HasKey(p => p.IdPlanning);

            modelBuilder.Entity<Planning>()
                .HasOne(p => p.Employe)
                .WithMany()
                .HasForeignKey(p => p.IdEmploye);

            // ✅ Pointage (One-to-Many with Employes)
            modelBuilder.Entity<Pointage>()
                .HasKey(p => p.IdPointage);

            modelBuilder.Entity<Pointage>()
                .HasOne(p => p.Employe)
                .WithMany()
                .HasForeignKey(p => p.IdEmploye);
        }
    }
}
