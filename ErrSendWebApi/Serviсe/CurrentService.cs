using ErrSendApplication.Interfaces;
using System.Security.Claims;

namespace ErrSendWebApi.Serviсe
{
    /// <summary>
    /// Клас для роботи з поточним користувачем системи
    /// </summary>
    public class CurrentService : ICurrentService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string? GetCurrentUserId()
        {
            return httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public string? GetCurrentUserName()
        {
            return httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        }

        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
