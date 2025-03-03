using HR_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.viewModels
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
