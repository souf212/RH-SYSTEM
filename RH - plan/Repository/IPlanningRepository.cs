using HR_Management_System.Models;
using HR_Management_System.viewModels;

namespace HR_Management_System.Repositories
{
    public interface IPlanningRepository
    {
        IEnumerable<Planning> GetAllWithDetails();
        Task<Planning?> GetByIdAsync(int id);
        Task AddAsync(Planning planning);
        Task UpdateAsync(Planning planning);
        Task DeleteAsync(Planning planning);
    }
}
