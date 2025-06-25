namespace ErrSendApplication.Interfaces
{
    public interface ICurrentService
    {
        string? GetCurrentUserId();
        string? GetCurrentUserName();
        bool IsAuthenticated { get; }
    }
}
