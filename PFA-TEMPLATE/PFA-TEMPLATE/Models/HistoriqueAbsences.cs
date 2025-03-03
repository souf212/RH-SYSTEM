using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.viewModels
{
    public class HistoriqueAbsences
    {
        [Key]
        public int IdHistoriqueAbsences { get; set; }

        // Foreign Key
        public int IdAbsences { get; set; }
        public Absences? Absence { get; set; }
    }
}
