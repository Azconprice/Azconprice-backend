using Domain.Entities;

namespace Application.Repositories
{
    public interface IExcelFileRecordRepository : IRepository<ExcelFileRecord>
    {
        Task<int> GetTotalCountAsync();
    }
}
