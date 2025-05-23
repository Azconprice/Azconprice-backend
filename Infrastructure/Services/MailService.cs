using Application.Models;
using Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MailService : IMailService
    {
        private readonly SMTPConfig _config;

        public MailService(SMTPConfig config)
        {
            _config = config;
        }

        public void SendConfirmationMessage(string email, string url)
        {
            using var smtp = new SmtpClient()
            {
                Host = _config.Host,
                Port = _config.Port,
                EnableSsl = _config.EnableSsl,
                Credentials = new NetworkCredential(_config.Username, _config.Password)
            };

            using var message = new MailMessage()
            {

                IsBodyHtml = true,
                Subject = "IUsta Email confirmation",
                Body = @$"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Confirmation</title>
</head>
<body style=""font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #1a1a1a; color: #ffffff;"">

    <table style=""max-width: 600px; margin: 0 auto; background-color: #1a1a1a; border-collapse: collapse;"">
        <tr>
            <td style=""padding: 20px; text-align: center; background-color: #b90000;"">
                <h1 style=""color: #ffffff;"">Email Confirmation</h1>
            </td>
        </tr>
        <tr>
            <td style=""padding: 20px;"">
                <p>Hello {email},</p>
                <p style=""color: #cccccc;"">Thank you for registering on our website. Please click the button below to confirm your email address:</p>
                <p style=""text-align: center;"">
                    <a href=""{url}"" style=""display: inline-block; padding: 10px 20px; background-color: #b90000; color: #ffffff; text-decoration: none; border-radius: 5px;"">Confirm Email</a>
                </p>
                <p style=""color: #cccccc;"">If you did not request this confirmation, you can ignore this email.</p>
                <p style=""color: #cccccc;"">Best regards,<br> The IUsta Team</p>
            </td>
        </tr>
        <tr>
            <td style=""padding: 20px; text-align: center; background-color: #1a1a1a;"">
                <p style=""margin: 0; color: #cccccc;"">© {DateTime.Now.Year} IUsta. All rights reserved.</p>
            </td>
        </tr>
    </table>

</body>
</html>"
            };

            message.From = new MailAddress(_config.Username);
            message.To.Add(new MailAddress(email));

            smtp.Send(message);
        }

        public void SendTaskAcceptanceMessage(string clientEmail, string workerEmail)
        {
            using var smtp = new SmtpClient()
            {
                Host = _config.Host,
                Port = _config.Port,
                EnableSsl = _config.EnableSsl,
                Credentials = new NetworkCredential(_config.Username, _config.Password)
            };

            using var userMessage = new MailMessage()
            {

                IsBodyHtml = true,
                Subject = "IUsta Request Acceptance",
                Body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Request Accepted</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f2f2f2;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #ff0000;
            color: #fff;
            text-align: center;
            padding: 20px;
        }}
        .message {{
            background-color: #fff;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }}
        p {{
            margin: 0;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Request Accepted</h1>
        </div>
        <div class=""message"">
            <p>Dear Client,</p>
            <p>We are pleased to inform you that your request has been accepted at {DateTime.Now} time.</p>
            <p>Thank you for choosing our services. If you have any questions or need further assistance, please don't hesitate to contact us.</p>
            <p>Best regards,</p>
            <p>© {DateTime.Now.Year} IUsta. All rights reserved</p>
        </div>
    </div>
</body>
</html>"
            };

            using var workerMessage = new MailMessage()
            {

                IsBodyHtml = true,
                Subject = "IUsta Request Acceptance",
                Body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Request Accepted</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f2f2f2;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #ff0000;
            color: #fff;
            text-align: center;
            padding: 20px;
        }}
        .message {{
            background-color: #fff;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }}
        p {{
            margin: 0;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Request Accepted</h1>
        </div>
        <div class=""message"">
            <p>Dear Worker,</p>
            <p>We want to inform you that you have accepted new request at {DateTime.Now} time.</p>
            <p>You can contact to your client through this mail address {clientEmail}.</p>
            <p>Thank you for your work on our service. If you have any questions or need further assistance, please don't hesitate to contact us.</p>
            <p>Best regards,</p>
            <p>© {DateTime.Now.Year} IUsta. All rights reserved</p>
        </div>
    </div>
</body>
</html>"
            };

            userMessage.From = new MailAddress(_config.Username);
            workerMessage.From = new MailAddress(_config.Username);
            workerMessage.To.Add(new MailAddress(workerEmail));
            userMessage.To.Add(new MailAddress(clientEmail));

            smtp.Send(workerMessage);
            smtp.Send(userMessage);
        }

        public void SendTaskRejectionMessage(string clientEmail)
        {
            using var smtp = new SmtpClient()
            {
                Host = _config.Host,
                Port = _config.Port,
                EnableSsl = _config.EnableSsl,
                Credentials = new NetworkCredential(_config.Username, _config.Password)
            };

            using var message = new MailMessage()
            {

                IsBodyHtml = true,
                Subject = "IUsta Request Acceptance",
                Body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Request Accepted</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f2f2f2;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #ff0000;
            color: #fff;
            text-align: center;
            padding: 20px;
        }}
        .message {{
            background-color: #fff;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }}
        p {{
            margin: 0;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Request Accepted</h1>
        </div>
        <div class=""message"">
            <p>Dear Client,</p>
            <p>We are unfortunately have to inform you that your request has been Rejected at {DateTime.Now} time.</p>
            <p>Please don't be upset and try to find another worker on our service. If you have any questions or need further assistance, please don't hesitate to contact us.</p>
            <p>Best regards,</p>
            <p>© {DateTime.Now.Year} IUsta. All rights reserved</p>
        </div>
    </div>
</body>
</html>"
            };

            message.From = new MailAddress(_config.Username);
            message.To.Add(new MailAddress(clientEmail));

            smtp.Send(message);
        }
    }
}
