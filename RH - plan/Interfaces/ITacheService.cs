using HR_Management_System.ViewModels;

namespace HR_Management_System.Services
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