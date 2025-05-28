namespace Application.Services
{
    public interface IAppLogger
    {
        Task LogAsync(string action, string? relatedEntityId, string? userId, string? userName, string? details = null);
    }
}
