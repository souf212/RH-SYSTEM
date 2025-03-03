using PFA_TEMPLATE.viewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.Models
{
    public class Employes
    {
        [Key]
        public int IdEmploye { get; set; }

        [ForeignKey("Utilisateurs")]
        public int IdUtilisateur { get; set; }

        // Propriétés calculées
        public string Nom => Utilisateur?.Nom ?? "Nom non disponible";
        public string Prenom => Utilisateur?.Prenom ?? "Prénom non disponible";
        public string NomComplet => Utilisateur != null ? $"{Utilisateur.Prenom} {Utilisateur.Nom}" : "Employé non chargé";

        // Navigation Property
        public virtual Utilisateur Utilisateur { get; set; }

        // Collections
        public virtual ICollection<Taches> Taches { get; set; }
        public virtual ICollection<Contrat> Contrats { get; set; }
        public virtual ICollection<Conges> Conges { get; set; }
        public virtual ICollection<Absences> Absences { get; set; }
        public virtual ICollection<ReconnaissanceFaciale> ReconnaissanceFaciales { get; set; }
        public virtual ICollection<Salaire> Salaires { get; set; }

        // Constructeur
        public Employes()
        {
            Taches = new HashSet<Taches>();
            Contrats = new HashSet<Contrat>();
            Conges = new HashSet<Conges>();
            Absences = new HashSet<Absences>();
            ReconnaissanceFaciales = new HashSet<ReconnaissanceFaciale>();
            Salaires = new HashSet<Salaire>();
        }

        // Méthode utilitaire
        public void AssignerUtilisateur(Utilisateur utilisateur)
        {
            if (utilisateur == null)
            {
                throw new ArgumentNullException(nameof(utilisateur));
            }

            Utilisateur = utilisateur;
            IdUtilisateur = utilisateur.Id;
        }
    }
}