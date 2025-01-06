using CoreAPI.Models;

namespace CoreAPI.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(User user);
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> UpdateUserAsync(string id, User user);
        Task<bool> DeleteUserAsync(string id);
    }
}
