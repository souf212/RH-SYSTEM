using PFA_TEMPLATE.viewModels;

namespace PFA_TEMPLATE.Repositories
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
