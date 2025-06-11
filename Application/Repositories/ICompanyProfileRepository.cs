using Domain.Entities;

namespace Application.Repositories
{
    public interface ICompanyProfileRepository : IRepository<CompanyProfile>
    {
        Task<CompanyProfile?> GetByUserIdAsync(string userId);
    }
}