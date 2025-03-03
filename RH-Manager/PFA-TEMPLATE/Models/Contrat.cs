using PFA_TEMPLATE.Models;
using System.ComponentModel.DataAnnotations;
namespace PFA_TEMPLATE.viewModels
{
    public class Contrat
    {
        [Key]
        public int IdContrat { get; set; }

        public decimal SalaireDeBase { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public string TypeContrat { get; set; } = string.Empty;

        // Foreign Key
        public int IdEmploye { get; set; }
        public Employes Employe { get; set; } = null!;

        // Navigation Property
        public ICollection<FicheDePaie> FichesDePaie { get; set; } = new List<FicheDePaie>();
    }
}
