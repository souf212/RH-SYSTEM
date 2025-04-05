using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Services
{
    public interface ITacheService
    {
        List<TachesVM> GetAllTaches();
        List<TachesVM> GetTachesByEmployee(string loggedInUser);
        public Task CreateTache(TachesVM tachesVM);
        TachesVM GetTacheById(int id);
        public Task UpdateTache(TachesVM tachesVM);
        public Task UpdateTacheStatus(TachesVM model);
        public Task DeleteTache(int id);

        List<EmployeDropdownVM> GetEmployesForDropdown();
    }
}