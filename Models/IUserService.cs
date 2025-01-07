using CoreAPI.Models;

namespace CoreAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task CreateUserAsync(User user);
        Task<User?> Authenticate(string email, string password);
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> UpdateUserAsync(string id, User user);
        Task<bool> DeleteUserAsync(string id);
    }
}
