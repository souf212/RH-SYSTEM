using PFA_TEMPLATE.viewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.Models
{
    public class Employes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdEmploye { get; set; }

        [ForeignKey("Utilisateur")]
        public int IdUtilisateur { get; set; }

        // 🟩 Nouveau champ : l'ID du responsable
        public int? ResponsableUtilisateurId { get; set; }

        [ForeignKey("ResponsableUtilisateurId")]
        public virtual Utilisateur? ResponsableUtilisateur { get; set; }


        // ✅ Propriétés calculées
        public string Nom => Utilisateur?.Nom ?? "Nom non disponible";
        public string Prenom => Utilisateur?.Prenom ?? "Prénom non disponible";
        public string NomComplet => Utilisateur != null ? $"{Utilisateur.Prenom} {Utilisateur.Nom}" : "Employé non chargé";

        // ✅ Navigation vers l'utilisateur lié
        public virtual Utilisateur Utilisateur { get; set; }

        // ✅ Collections
        public virtual ICollection<Taches> Taches { get; set; }
        public virtual ICollection<Contrat> Contrats { get; set; }
        public virtual ICollection<Conges> Conges { get; set; }
        public virtual ICollection<Absences> Absences { get; set; }
        public virtual ICollection<ReconnaissanceFaciale> ReconnaissanceFaciales { get; set; }
        public virtual ICollection<Salaire> Salaires { get; set; }
        public virtual ICollection<Pointage> Pointages { get; set; }

        public Employes()
        {
            Taches = new HashSet<Taches>();
            Contrats = new HashSet<Contrat>();
            Conges = new HashSet<Conges>();
            Absences = new HashSet<Absences>();
            ReconnaissanceFaciales = new HashSet<ReconnaissanceFaciale>();
            Salaires = new HashSet<Salaire>();
            Pointages = new HashSet<Pointage>();
        }

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
