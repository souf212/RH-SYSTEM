using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.viewModels
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
