using CoreAPI.Models;

namespace CoreAPI.Repositories
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User?> GetByIdAsync(string id);
        Task<User?> UpdateAsync(string id, User user);
        Task<bool> DeleteAsync(string id);
    }
}
