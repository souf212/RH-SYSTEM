using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Services
{
    public interface IUserService
    {
        Task<UserVM> GetUserByIdAsync(int id);
        Task<IEnumerable<UserVM>> GetAllUsersAsync();
        Task<UserVM> CreateUserAsync(UserVM userVM);
        Task<bool> UpdateUserAsync(UserVM userVM);
        Task<bool> DeleteUserAsync(int id);

    }
}