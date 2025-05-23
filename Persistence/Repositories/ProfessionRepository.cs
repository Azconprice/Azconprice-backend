using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class ProfessionRepository(AppDbContext context) : Repository<Profession>(context), IProfessionRepository
    {
    }
}
