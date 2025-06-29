using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class OtpVerificationRepository(AppDbContext context) : Repository<OtpVerification>(context), IOtpVerificationRepository
    {
        private readonly AppDbContext _context = context;
        public async Task<OtpVerification?> GetLatestUnverifiedCodeAsync(string contact, string code)
        {
            return await _context.OtpVerifications
                .Where(v => v.Contact == contact && v.Code == code && !v.IsVerified)
                .OrderByDescending(v => v.ExpirationDate)
                .FirstOrDefaultAsync();
        }

        public async Task<OtpVerification?> GetValidOtpAsync(string contact, ContactType contactType, string code)
        {
            return await _context.OtpVerifications
                .Where(v => v.Contact == contact
                            && v.ContactType == contactType
                            && v.Code == code
                            && !v.IsVerified
                            && v.ExpirationDate > DateTime.UtcNow)
                .OrderByDescending(v => v.ExpirationDate)
                .FirstOrDefaultAsync();
        }

        public async Task<OtpVerification?> GetLatestVerifiedCodeAsync(string contact, ContactType contactType)
        {
            // Only consider OTPs verified in the last 15 minutes
            var minDate = DateTime.UtcNow.AddMinutes(-15);
            return await _context.OtpVerifications
                .Where(v => v.Contact == contact
                            && v.ContactType == contactType
                            && v.IsVerified
                            && v.ExpirationDate > minDate)
                .OrderByDescending(v => v.ExpirationDate)
                .FirstOrDefaultAsync();
        }

    }
}
