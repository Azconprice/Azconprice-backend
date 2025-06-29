using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface IOtpVerificationRepository : IRepository<OtpVerification>
    {
        Task<OtpVerification?> GetLatestUnverifiedCodeAsync(string contact, string code);
        Task<OtpVerification?> GetValidOtpAsync(string contact, ContactType contactType, string code);
        Task<OtpVerification?> GetLatestVerifiedCodeAsync(string contact, ContactType contactType);
    }
}
