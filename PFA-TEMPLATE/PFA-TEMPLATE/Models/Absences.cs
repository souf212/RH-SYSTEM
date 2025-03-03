using PFA_TEMPLATE.Models;
using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.viewModels
{
    public class Absences
    {
        [Key]
        public int IdAbsences { get; set; }

        public DateTime DateAbsence { get; set; }
        public string? Justification { get; set; }

        // Foreign Key
        public int IdEmploye { get; set; }
        public Employes Employe { get; set; } = null!;
    }
}
