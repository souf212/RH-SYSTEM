using PFA_TEMPLATE.viewModels;
using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Mappers
{
    public static class PlanningMapper
    {
        /// <summary>
        /// Maps a Planning entity to a PlanningViewModel.
        /// </summary>
        /// <param name="planning">The Planning entity.</param>
        /// <returns>A PlanningViewModel object.</returns>
        public static PlanningViewModel ToViewModel(Planning planning)
        {
            return new PlanningViewModel
            {

                IdPlanning = planning.IdPlanning,
                DateDebut = planning.DateDebut,
                DateFin = planning.DateFin,
                Statut = planning.Statut,
                EmployeName = $"{planning.Employe.Nom} {planning.Employe.Prenom}",
                IdEmploye = planning.IdEmploye
            };
        }

        /// <summary>
        /// Maps a list of Planning entities to a list of PlanningViewModels.
        /// </summary>
        /// <param name="plannings">The list of Planning entities.</param>
        /// <returns>A list of PlanningViewModel objects.</returns>
        public static List<PlanningViewModel> ToViewModels(IEnumerable<Planning> plannings)
        {
            return plannings.Select(ToViewModel).ToList();
        }

        /// <summary>
        /// Maps a PlanningViewModel to a Planning entity.
        /// </summary>
        /// <param name="viewModel">The PlanningViewModel object.</param>
        /// <returns>A Planning entity.</returns>
        public static Planning ToEntity(PlanningViewModel viewModel)
        {
            return new Planning
            {
                IdPlanning = viewModel.IdPlanning ?? 0,
                DateDebut = viewModel.DateDebut,
                DateFin = viewModel.DateFin,
                Statut = viewModel.Statut,
                IdEmploye = viewModel.IdEmploye
            };
        }
    }
}
