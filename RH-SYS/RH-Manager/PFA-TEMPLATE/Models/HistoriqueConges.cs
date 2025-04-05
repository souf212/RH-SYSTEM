using PFA_TEMPLATE.Models;
using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.viewModels
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
