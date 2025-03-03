using PFA_TEMPLATE.Models;
using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.ViewModels
{
    public class PlanningViewModel
    {
        public int? IdPlanning { get; set; } // Optional for create operations

        [Required]
        [Display(Name = "Date Début")]
        public DateTime DateDebut { get; set; }

        [Required]
        [Display(Name = "Date Fin")]
        public DateTime DateFin { get; set; }

        [Required]
        [Display(Name = "Statut")]
        public string Statut { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Employé")]
        public int IdEmploye { get; set; }

        public List<Employes>? EmployesList { get; set; } // For dropdown list
        public string EmployeName { get; internal set; }
    }
}
