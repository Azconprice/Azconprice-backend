using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IMailService
    {
        public void SendConfirmationMessage(string email, string url);
        public void SendTaskAcceptanceMessage(string clientEmail, string workerEmail);
        public void SendTaskRejectionMessage(string clientEmail);
    }
}
