using HR_Management_System.ViewModels;

namespace HR_Management_System.Mappers
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
