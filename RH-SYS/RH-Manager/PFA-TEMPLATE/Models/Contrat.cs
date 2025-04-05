// 📁 Models/Contrat.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PFA_TEMPLATE.viewModels;

namespace PFA_TEMPLATE.Models
{
    public class Contrat
    {
        [Key]
        public int IdContrat { get; set; }

        public decimal SalaireDeBase { get; set; }

        public DateTime DateDebut { get; set; }

        public DateTime? DateFin { get; set; }

        [Required]
        public string TypeContrat { get; set; } = string.Empty; // "CDI" ou "CDD"

        public string EtatContrat { get; set; } = "En attente"; // En attente, Affecté, Expiré

        public int? IdUtilisateur { get; set; }

        [ForeignKey("IdUtilisateur")]
        public Utilisateur? Utilisateur { get; set; }

        public ICollection<FicheDePaie> FichesDePaie { get; set; } = new List<FicheDePaie>();
    }

}
