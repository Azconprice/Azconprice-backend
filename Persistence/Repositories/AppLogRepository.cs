using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class AppLogRepository(AppDbContext context) : Repository<AppLog>(context), IAppLogRepository
    {
    }
}
