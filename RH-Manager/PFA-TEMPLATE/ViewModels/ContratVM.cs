// ViewModels/ContratVM.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.ViewModels
{
    public class ContratVM
    {
        public int? IdUtilisateur { get; set; }

        [Required]
        public decimal SalaireDeBase { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        public DateTime? DateFin { get; set; }

        [Required]
        public string TypeContrat { get; set; } = string.Empty;
    }

}
