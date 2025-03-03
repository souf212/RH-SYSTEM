using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.ViewModels
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
