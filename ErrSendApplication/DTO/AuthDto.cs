using System.ComponentModel.DataAnnotations;

namespace ErrSendApplication.DTO
{
    /// <summary>
    /// Модель запиту на автентифікацію
    /// </summary>
    public class AuthRequest
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }

    /// <summary>
    /// Модель відповіді з JWT токеном
    /// </summary>
    public class AuthResponse
    {
        public required string Token { get; set; }
    }

    /// <summary>
    /// Модель для реєстрації нового користувача
    /// </summary>
    public class RegisterRequest
    {
        [Required]
        [MinLength(3)]
        public required string Username { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
    }

    /// <summary>
    /// Модель для генерації тестового JWT токена
    /// </summary>
    public class GenerateTokenRequest
    {
        [Required]
        public required string Username { get; set; }

        public List<string>? Roles { get; set; }

        public int ExpiryMinutes { get; set; } = 60;

        public Dictionary<string, string>? CustomClaims { get; set; }
    }

    /// <summary>
    /// Модель користувача в системі
    /// </summary>
    public class User
    {
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 