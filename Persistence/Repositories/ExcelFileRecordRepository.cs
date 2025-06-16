using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class ExcelFileRecordRepository(AppDbContext context) : Repository<ExcelFileRecord>(context), IExcelFileRecordRepository
    {
        private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.ExcelFileRecords.CountAsync();
        }
    }
}
