using Application.Repositories;
using Application.Services;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class AppLogger(IAppLogRepository repository) : IAppLogger
    {
        private readonly IAppLogRepository _repository = repository;

        public async Task LogAsync(string action, string? relatedEntityId, string? userId, string? userName, string? details = null)
        {
            var log = new AppLog
            {
                Action = action,
                RelatedEntityId = relatedEntityId,
                UserId = userId,
                UserName = userName,
                Details = details
            };
            await _repository.AddAsync(log);
            await _repository.SaveChangesAsync();
        }
    }
}
