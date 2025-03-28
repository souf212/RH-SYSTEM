using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Mappers
{
    public static class TacheMapper
    {
        public static TachesVM ToViewModel(Taches tache)
        {
            return new TachesVM
            {
                Id = tache.IdTaches,
                Title = tache.Titre,
                Description = tache.Description,
                AssignedTo = tache.IdEmploye.ToString(),
                Status = tache.Statut
            };
        }

        public static Taches ToEntity(TachesVM tachesVM)
        {
            return new Taches
            {
                IdTaches = tachesVM.Id,
                Titre = tachesVM.Title,
                Description = tachesVM.Description,
                Statut = tachesVM.Status,
                IdEmploye = int.Parse(tachesVM.AssignedTo),
                CreatedAt = DateTime.Now,
                LastModified = DateTime.Now
            };
        }
    }
}