using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class WorkerFunctionRepository(AppDbContext context) : Repository<WorkerFunction>(context), IWorkerFunctionRepository
    {
    }
}
