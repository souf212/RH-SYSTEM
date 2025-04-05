using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.ViewModels
{
    public class EchangeTachesVM
    {
        public int IdEchange { get; set; }

        [Required(ErrorMessage = "La tâche du demandeur est requise")]
        public int IdTacheDemandeur { get; set; }
        public string? TitreTacheDemandeur { get; set; }

        [Required(ErrorMessage = "La tâche du destinataire est requise")]
        public int IdTacheDestinataire { get; set; }
        public string? TitreTacheDestinataire { get; set; }

        [Required(ErrorMessage = "L'employé demandeur est requis")]
        public int IdEmployeDemandeur { get; set; }
        public string? NomEmployeDemandeur { get; set; }

        [Required(ErrorMessage = "L'employé destinataire est requis")]
        public int IdEmployeDestinataire { get; set; }
        public string? NomEmployeDestinataire { get; set; }

        public string Statut { get; set; } = "EnAttente";

        public DateTime DateDemande { get; set; } = DateTime.Now;
        public DateTime? DateReponse { get; set; }

        public string? RaisonRefus { get; set; }
    }
}