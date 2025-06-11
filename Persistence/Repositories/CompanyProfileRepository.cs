using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class CompanyProfileRepository(AppDbContext context) : Repository<CompanyProfile>(context), ICompanyProfileRepository
    {
        public Task<CompanyProfile?> GetByUserIdAsync(string userId) => GetAsync(cp => cp.UserId == (userId));
    }
}