using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.Models
{
    public class Taches
    {
        [Key]

        public int IdTaches { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères")]
        public string Titre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La description est obligatoire")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le statut est obligatoire")]
        public string Statut { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date limite est obligatoire")]
        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime LastModified { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "L'employé est obligatoire")]
        public int IdEmploye { get; set; }

        [ForeignKey("IdEmploye")]
        public virtual Employes? Employe { get; set; }

    }
}