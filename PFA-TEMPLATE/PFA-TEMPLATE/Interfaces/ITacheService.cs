using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Services
{
    public interface ITacheService
    {
        List<TachesVM> GetAllTaches();
        List<TachesVM> GetTachesByEmployee(string loggedInUser);
        void CreateTache(TachesVM tachesVM);
        TachesVM GetTacheById(int id);
        void UpdateTache(TachesVM tachesVM);
        void UpdateTacheStatus(TachesVM model);
        void DeleteTache(int id);
        List<EmployeDropdownVM> GetEmployesForDropdown();
    }
}