using HR_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.viewModels
{
    public class HistoriqueConges
    {
        [Key]
        public int IdHistoriqueConges { get; set; }

        // Foreign Key
        public int IdConges { get; set; }
        public Conges? Conges { get; set; }
    }
}
