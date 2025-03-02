using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.ViewModels
{
    public class DemandeCongeViewModels
    {

        [Required]
        public string Motif { get; set; } = string.Empty;
        [Required]
        public DateTime DateDebut { get; set; }
        [Required]
        public DateTime DateFin { get; set; }
    }
}
