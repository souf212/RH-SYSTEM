using System;
using System.ComponentModel.DataAnnotations;
using PFA_TEMPLATE.Models;

namespace PFA_TEMPLATE.ViewModels
{
    public class DemandeCongeViewModel
    {
        public int IdEmploye { get; set; }

        [Required(ErrorMessage = "Le motif est obligatoire")]
        public string Motif { get; set; }

        [Required(ErrorMessage = "La date de début est obligatoire")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Required(ErrorMessage = "La date de fin est obligatoire")]
        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; }

        // Pour les suggestions si conflit
        public bool PeriodeSuggeree { get; set; } = false;

        public DateTime? SuggestionDateDebut { get; set; }

        public DateTime? SuggestionDateFin { get; set; }

        public List<Conges>? ListeDemandes { get; set; }

        public decimal? SoldeCongesPayesRestants { get; set; }
        public decimal? SoldeMaladieRestants { get; set; }


    }
}
