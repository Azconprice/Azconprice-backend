using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class RequestRepository(AppDbContext context) : Repository<Request>(context), IRequestRepository
    {
    }
}
