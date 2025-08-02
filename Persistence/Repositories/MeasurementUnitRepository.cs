using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class MeasurementUnitRepository(AppDbContext context) : Repository<MeasurementUnit>(context), IMeasurementUnitRepository
    {
    }
}
