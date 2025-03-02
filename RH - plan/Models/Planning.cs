using HR_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.viewModels
{
    public class Planning
    {
        [Key]
        public int IdPlanning { get; set; }

        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public required string Statut { get; set; }

        // Foreign Key
        public int IdEmploye { get; set; }
        public Employes Employe { get; set; } = null!;
    }
}
