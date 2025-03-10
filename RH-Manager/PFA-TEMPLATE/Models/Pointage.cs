using PFA_TEMPLATE.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int IdEmploye { get; set; }

        public virtual Employes Employe { get; set; }  // ✅ Ensure it's correctly mapped
    }

}
