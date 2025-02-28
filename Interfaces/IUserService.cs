using HR_Management_System.ViewModels;

namespace HR_Management_System.Services
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