using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CEGA.Servicios
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string html);
    }

    public class MailKitEmailSender : IEmailSender
    {
        private readonly IConfiguration _cfg;

        public MailKitEmailSender(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public async Task SendAsync(string to, string subject, string html)
        {
            var from = _cfg["Email:From"];
            var pass = _cfg["Email:Password"];
            var host = _cfg["Email:Host"] ?? "smtp.office365.com";
            var port = int.TryParse(_cfg["Email:Port"], out var p) ? p : 587;

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("CEGA", from));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = html };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(from, pass);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }
    }
}
