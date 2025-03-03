using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.viewModels
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
