using HR_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.viewModels
{
    public class Salaire
    {
        [Key]
        public int IdSalaire { get; set; }

        public required string Grade { get; set; }
        public required string Competence { get; set; }
        public decimal MontantBrut { get; set; }
        public decimal MontantNet { get; set; }

        // Foreign Key
        public int IdEmploye { get; set; }
        public Employes Employe { get; set; } = null!;
    }
}
