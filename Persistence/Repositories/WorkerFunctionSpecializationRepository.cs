using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class WorkerFunctionSpecializationRepository(AppDbContext context) : Repository<WorkerFunctionSpecialization>(context), IWorkerFunctionSpecializationRepository
    {
    }
}
