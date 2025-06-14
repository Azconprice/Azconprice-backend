using Application.Models;
using Application.Services;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace Infrastructure.Services
{
    public class TwilioService : ISMSService
    {
        private readonly TwilioOptions _options;

        public TwilioService(TwilioOptions options)
        {
            _options = options;
            TwilioClient.Init(_options.AccountSid, _options.AuthToken);
        }

        public async Task<string> SendVerificationCodeAsync(string phoneNumber)
        {
            var verification = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: "sms",
                pathServiceSid: _options.VerifyServiceSid);

            return verification.Status; // e.g., "pending", "canceled", "failed"
        }

        public async Task<string> VerifyCodeAsync(string phoneNumber, string code)
        {
            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: code,
                pathServiceSid: _options.VerifyServiceSid);

            return verificationCheck.Status; // e.g., "approved", "pending", "canceled"
        }
    }
}
