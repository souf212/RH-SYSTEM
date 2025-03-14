using System;
namespace PFA_TEMPLATE.ViewModels
{
    public class PointageViewModel
    {
        public int IdEmploye { get; set; }
        public string NomEmploye { get; set; }
        public DateTime? HeureEntree { get; set; }
        public DateTime? HeureSortie { get; set; }
        public string Statut { get; set; }  // Présent ou Absent
    }
}

