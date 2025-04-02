// 📁 Models/FicheDePaie.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PFA_TEMPLATE.Models;

namespace PFA_TEMPLATE.Models
{
    public class FicheDePaie
    {
        [Key]
        public int IdFicheDePaie { get; set; }

        public DateTime Date { get; set; }

        public decimal SalaireNet { get; set; }

        // FK vers Contrat
        public int IdContrat { get; set; }

        [ForeignKey("IdContrat")]
        public Contrat Contrat { get; set; } = null!;
    }
}
