using CoreAPI.Models;

namespace CoreAPI.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
