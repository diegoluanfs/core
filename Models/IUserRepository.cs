using CoreAPI.Models;

namespace CoreAPI.Repositories
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailOrPhoneAsync(string email, string phoneNumber);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> UpdateAsync(string id, User user);
        Task<bool> DeleteAsync(string id);
    }
}
