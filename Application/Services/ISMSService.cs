namespace Application.Services
{
    public interface ISMSService
    {
        Task<string> SendVerificationCodeAsync(string phoneNumber);
        Task<string> SendVerificationCodeAsync(string phoneNumber,string code);
        Task<string> VerifyCodeAsync(string phoneNumber, string code);
    }
}
