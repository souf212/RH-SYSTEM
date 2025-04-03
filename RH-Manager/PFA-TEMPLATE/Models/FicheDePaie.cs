using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.Models
{
    public class FicheDePaie
    {
        [Key]
        public int IdFicheDePaie { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public decimal SalaireNet { get; set; }

        // 🔗 Lien avec le contrat
        public int IdContrat { get; set; }

        [ForeignKey("IdContrat")]
        public Contrat Contrat { get; set; } = null!;
    }
}
