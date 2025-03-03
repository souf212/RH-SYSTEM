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
        public DbSet<CongeBalance> CongeBalances { get; set; } // Add the new entity

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Utilisateur
            modelBuilder.Entity<Utilisateur>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Conges>()
        .HasOne(c => c.Employe) // Conges a un Employe
        .WithMany(e => e.Conges) // Employe a plusieurs Conges
        .HasForeignKey(c => c.IdEmploye); // Clé étrangère


            modelBuilder.Entity<CongeBalance>()
         .HasOne(cb => cb.Employe)
         .WithMany()
         .HasForeignKey(cb => cb.IdEmploye)
         .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CongeBalance>()
                .HasIndex(cb => new { cb.IdEmploye, cb.Annee })
                .IsUnique();
            //planning

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

            modelBuilder.Entity<PlageHoraire>()
                .HasOne(p => p.EmploiDuTemps)
                .WithMany(e => e.PlagesHoraires)
                .HasForeignKey(p => p.EmploiDuTempsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContraintesPlanning>()
           .Property(p => p.Id)
           .ValueGeneratedOnAdd();
            // Propriété de fitness pour l'algorithme génétique
            modelBuilder.Entity<EmploiDuTemps>()
                .Ignore(e => e.Fitness);

            // Employes
            modelBuilder.Entity<Employes>()
                .HasKey(e => e.IdEmploye);

            modelBuilder.Entity<Employes>()
                .HasOne(e => e.Utilisateur)
                .WithOne()
                .HasForeignKey<Employes>(e => e.IdUtilisateur)
                .OnDelete(DeleteBehavior.Restrict);

            // Taches
            modelBuilder.Entity<Taches>()
                .HasKey(t => t.IdTaches);

            modelBuilder.Entity<Taches>()
                .HasOne(t => t.Employe)
                .WithMany(e => e.Taches)
                .HasForeignKey(t => t.IdEmploye)
                .OnDelete(DeleteBehavior.Restrict);

            // Administrateur
            modelBuilder.Entity<Administrateur>()
                .HasKey(a => a.IdAdmin);

            modelBuilder.Entity<Administrateur>()
                .HasOne(a => a.Utilisateur)
                .WithOne()
                .HasForeignKey<Administrateur>(a => a.IdUtilisateur);

            // Contrats
            modelBuilder.Entity<Contrat>()
                .HasKey(c => c.IdContrat);

            modelBuilder.Entity<Contrat>()
                .HasOne(c => c.Employe)
                .WithMany(e => e.Contrats)
                .HasForeignKey(c => c.IdEmploye);

            // Conges
            modelBuilder.Entity<Conges>()
                .HasKey(c => c.IdConges);

            modelBuilder.Entity<Conges>()
                .HasOne(c => c.Employe)
                .WithMany(e => e.Conges)
                .HasForeignKey(c => c.IdEmploye);

            // Absences
            modelBuilder.Entity<Absences>()
                .HasKey(a => a.IdAbsences);

            modelBuilder.Entity<Absences>()
                .HasOne(a => a.Employe)
                .WithMany(e => e.Absences)
                .HasForeignKey(a => a.IdEmploye);

            // HistoriqueAbsences
            modelBuilder.Entity<HistoriqueAbsences>()
                .HasKey(h => h.IdHistoriqueAbsences);

            modelBuilder.Entity<HistoriqueAbsences>()
                .HasOne(h => h.Absence)
                .WithMany()
                .HasForeignKey(h => h.IdAbsences);

            // HistoriqueConges
            modelBuilder.Entity<HistoriqueConges>()
                .HasKey(h => h.IdHistoriqueConges);

            modelBuilder.Entity<HistoriqueConges>()
                .HasOne(h => h.Conges)
                .WithMany()
                .HasForeignKey(h => h.IdConges);

            // ReconnaissanceFaciale
            modelBuilder.Entity<ReconnaissanceFaciale>()
                .HasKey(r => r.IdReconnaissanceFaciale);

            modelBuilder.Entity<ReconnaissanceFaciale>()
                .HasOne(r => r.Employe)
                .WithMany(e => e.ReconnaissanceFaciales)
                .HasForeignKey(r => r.IdEmploye);

            // Salaire
            modelBuilder.Entity<Salaire>()
                .HasKey(s => s.IdSalaire);

            modelBuilder.Entity<Salaire>()
                .HasOne(s => s.Employe)
                .WithMany(e => e.Salaires)
                .HasForeignKey(s => s.IdEmploye);

            // FicheDePaie
            modelBuilder.Entity<FicheDePaie>()
                .HasKey(f => f.IdFicheDePaie);

            modelBuilder.Entity<FicheDePaie>()
                .HasOne(f => f.Contrat)
                .WithMany(c => c.FichesDePaie)
                .HasForeignKey(f => f.IdContrat);

            // Planning
            modelBuilder.Entity<Planning>()
                .HasKey(p => p.IdPlanning);

            modelBuilder.Entity<Planning>()
                .HasOne(p => p.Employe)
                .WithMany()
                .HasForeignKey(p => p.IdEmploye);

            // Pointage
            modelBuilder.Entity<Pointage>()
                .HasKey(p => p.IdPointage);

            modelBuilder.Entity<Pointage>()
                .HasOne(p => p.Employe)
                .WithMany()
                .HasForeignKey(p => p.IdEmploye);
        }
    }
}
