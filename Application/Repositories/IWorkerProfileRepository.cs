using Domain.Entities;

namespace Application.Repositories
{
    public interface IWorkerProfileRepository : IRepository<WorkerProfile>
    {
        public Task<WorkerProfile?> GetByUserIdAsync(string userId);
    }
}