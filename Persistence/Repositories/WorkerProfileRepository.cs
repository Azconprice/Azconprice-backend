using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class WorkerProfileRepository(AppDbContext context) : Repository<WorkerProfile>(context), IWorkerProfileRepository
    {
        public Task<WorkerProfile?> GetByUserIdAsync(string userId) => GetAsync(wp => wp.UserId == (userId));
    }
}