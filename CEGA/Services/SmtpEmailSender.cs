
using System.Net;
using System.Net.Mail;

namespace CEGA.Services
{
    public class SmtpEmailSender : CEGA.Services.IEmailSender 
    {
        private readonly IConfiguration _cfg;
        private readonly ILogger<SmtpEmailSender> _log;

        public SmtpEmailSender(IConfiguration cfg, ILogger<SmtpEmailSender> log)
        {
            _cfg = cfg; _log = log;
        }

        public async Task SendEmailAsync(string to, string subject, string html)
        {
            var host = _cfg["Email:Host"];
            var port = int.Parse(_cfg["Email:Port"] ?? "587");
            var user = _cfg["Email:User"];
            var pass = _cfg["Email:Pass"];
            var from = _cfg["Email:From"];
            var ssl = bool.Parse(_cfg["Email:EnableSsl"] ?? "true");

            using var msg = new MailMessage(from, to, subject, html) { IsBodyHtml = true };
            using var smtp = new SmtpClient(host, port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = ssl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await smtp.SendMailAsync(msg);
        }
    }
}
