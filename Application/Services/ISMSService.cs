namespace Application.Services
{
    public interface ISMSService
    {
        Task<string> SendVerificationCodeAsync(string phoneNumber);
        Task<string> VerifyCodeAsync(string phoneNumber, string code);
    }
}
