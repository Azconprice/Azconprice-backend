using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ISMSService
    {
        Task<string> SendVerificationCodeAsync(string phoneNumber);
        Task<string> VerifyCodeAsync(string phoneNumber, string code);
    }
}
