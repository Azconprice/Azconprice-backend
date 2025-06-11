using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class SalesCategoryRepository(AppDbContext context) : Repository<SalesCategory>(context), ISalesCategoryRepository
    {
    }
}
