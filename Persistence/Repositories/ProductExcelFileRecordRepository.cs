using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class ProductExcelFileRecordRepository(AppDbContext context) : Repository<ProductExcelFileRecord>(context), IProductExcelFileRecordRepository
    {
    }
}
