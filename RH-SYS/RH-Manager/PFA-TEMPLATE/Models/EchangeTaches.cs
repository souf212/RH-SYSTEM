using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.Models
{
    public class EchangeTaches
    {
        [Key]
        public int IdEchange { get; set; }

        [Required]
        public int IdTacheDemandeur { get; set; }
        [ForeignKey("IdTacheDemandeur")]
        public virtual Taches? TacheDemandeur { get; set; }

        [Required]
        public int IdTacheDestinataire { get; set; }
        [ForeignKey("IdTacheDestinataire")]
        public virtual Taches? TacheDestinataire { get; set; }

        [Required]
        public int IdEmployeDemandeur { get; set; }
        [ForeignKey("IdEmployeDemandeur")]
        public virtual Employes? EmployeDemandeur { get; set; }

        [Required]
        public int IdEmployeDestinataire { get; set; }
        [ForeignKey("IdEmployeDestinataire")]
        public virtual Employes? EmployeDestinataire { get; set; }

        [Required]
        public string Statut { get; set; } = "EnAttente"; // EnAttente, Accepte, Refuse

        public DateTime DateDemande { get; set; } = DateTime.Now;
        public DateTime? DateReponse { get; set; }

        public string? RaisonRefus { get; set; }
    }
}