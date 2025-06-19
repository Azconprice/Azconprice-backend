using Domain.Entities;

namespace Application.Repositories
{
    public interface IPhoneVerificationRepository : IRepository<PhoneVerification>
    {
        Task<PhoneVerification?> GetLatestUnverifiedCodeAsync(string phoneNumber, string code);
    }
}
