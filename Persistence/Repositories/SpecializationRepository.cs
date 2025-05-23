using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class SpecializationRepository(AppDbContext context) : Repository<Specialization>(context), ISpecializationRepository
    {
    }
}
