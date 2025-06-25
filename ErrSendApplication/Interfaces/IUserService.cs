using ErrSendApplication.DTO;

namespace ErrSendApplication.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(RegisterRequest request);
        Task<User?> ValidateUserAsync(string username, string password);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UserExistsAsync(string username);
    }
} 