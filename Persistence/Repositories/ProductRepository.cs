using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class ProductRepository(AppDbContext context) : Repository<Product>(context), IProductRepository
    {
    }
}
