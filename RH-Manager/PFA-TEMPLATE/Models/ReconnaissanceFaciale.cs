using PFA_TEMPLATE.Models;
using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.viewModels
{
    public class ReconnaissanceFaciale
    {
        [Key]
        public int IdReconnaissanceFaciale { get; set; }

        public string? ImageFaciale { get; set; }
        public DateTime HeureDetectee { get; set; }

        // Foreign Key
        public int IdEmploye { get; set; }
        public Employes Employe { get; set; } = null!;
    }

}
