using ErrSendApplication.DTO;
using ErrSendApplication.Interfaces;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace ErrSendWebApi.Serviсe
{
    public class UserService : IUserService
    {
        private readonly ConcurrentDictionary<string, User> users = new();

        public UserService()
        {
            // Додаємо стандартного адміна для зворотної сумісності
            RegisterUserAsync(new RegisterRequest 
            { 
                Username = "admin", 
                Password = "admin",
                Roles = ["Admin"]
            }).Wait();
        }

        public Task<bool> RegisterUserAsync(RegisterRequest request)
        {
            if (users.ContainsKey(request.Username))
            {
                return Task.FromResult(false);
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                Email = request.Email,
                Roles = request.Roles ?? new List<string> { "User" }
            };

            return Task.FromResult(users.TryAdd(request.Username, user));
        }

        public Task<User?> ValidateUserAsync(string username, string password)
        {
            if (users.TryGetValue(username, out var user))
            {
                if (VerifyPassword(password, user.PasswordHash))
                {
                    return Task.FromResult<User?>(user);
                }
            }
            return Task.FromResult<User?>(null);
        }

        public Task<User?> GetUserByUsernameAsync(string username)
        {
            users.TryGetValue(username, out var user);
            return Task.FromResult(user);
        }

        public Task<bool> UserExistsAsync(string username)
        {
            return Task.FromResult(users.ContainsKey(username));
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "ErrorSender_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
} 