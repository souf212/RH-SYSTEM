using HR_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.viewModels
{
    public class Pointage
    {
        [Key]
        public int IdPointage { get; set; }

        public DateTime HeureEntree { get; set; }
        public DateTime HeureSortie { get; set; }

        // Foreign Key
        public int IdEmploye { get; set; }
        public Employes Employe { get; set; } = null!;
    }

}
