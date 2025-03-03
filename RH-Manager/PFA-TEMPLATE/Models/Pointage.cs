using PFA_TEMPLATE.Models;
using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.viewModels
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
