using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.viewModels
{
    public class FicheDePaie
    {
        [Key]
        public int IdFicheDePaie { get; set; }

        public DateTime Date { get; set; }
        public decimal SalaireNet { get; set; }

        // Foreign Key
        public int IdContrat { get; set; }
        public Contrat Contrat { get; set; } = null!;
    }
}
