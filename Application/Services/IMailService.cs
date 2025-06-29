using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IMailService
    {
        void SendConfirmationMessage(string email, string url);
        void SendPasswordResetMessage(string email, string content);
        void SendTaskAcceptanceMessage(string clientEmail, string workerEmail);
        void SendTaskRejectionMessage(string clientEmail);
    }
}
