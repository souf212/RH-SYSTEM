using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.Models
{
    public class CongeBalance
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employes")]
        public int IdEmploye { get; set; }

        public int Annee { get; set; }

        public decimal JoursCongesPayesTotal { get; set; }

        public decimal JoursCongesPayesUtilises { get; set; }

        public decimal JoursMaladieTotal { get; set; }

        public decimal JoursMaladieUtilises { get; set; }

        // Navigation property
        public virtual Employes Employe { get; set; }

        // Calculated properties
        [NotMapped]
        public decimal JoursCongesPayesRestants => JoursCongesPayesTotal - JoursCongesPayesUtilises;

        [NotMapped]
        public decimal JoursMaladieRestants => JoursMaladieTotal - JoursMaladieUtilises;
    }
}