using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class PhoneVerificationRepository(AppDbContext context) : Repository<PhoneVerification>(context), IPhoneVerificationRepository
    {
        private readonly AppDbContext _context = context;
        public async Task<PhoneVerification?> GetLatestUnverifiedCodeAsync(string phoneNumber, string code)
        {
            return await _context.PhoneVerifications
                .Where(v => v.PhoneNumber == phoneNumber && v.Code == code && !v.IsVerified)
                .OrderByDescending(v => v.ExpirationDate)
                .FirstOrDefaultAsync();
        }

    }
}
