using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Mappers
{
    public class DemandeCongeMapper
    {
        public static Conges GetCongesFromDemandeCongeViewModel(DemandeCongeViewModels vm)
        {
            return new Conges
            {
                Motif = vm.Motif,
                DateDebut = vm.DateDebut,
                DateFin = vm.DateFin,
            };
        }


    }
}
