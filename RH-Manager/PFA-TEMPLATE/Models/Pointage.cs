using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PFA_TEMPLATE.Models;

namespace PFA_TEMPLATE.viewModels
{
    public class Pointage
    {
        [Key]
        public int IdPointage { get; set; }

        [Required]
        public DateTime HeureEntree { get; set; }

        public DateTime? HeureSortie { get; set; }

        [ForeignKey("Employe")]
        [Required]
        public int IdEmploye { get; set; }

        // 🔥 **Change: Make `Employe` Optional**
        public virtual Employes? Employe { get; set; }
    }
}
