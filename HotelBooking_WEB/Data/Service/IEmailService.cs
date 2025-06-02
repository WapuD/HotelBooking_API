using MailKit.Security;

namespace HotelBooking_WEB.Data.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("HotelBooking", emailSettings["SmtpUser"]));
            message.To.Add(new MimeKit.MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new MimeKit.TextPart("html") { Text = body };

            using var client = new MailKit.Net.Smtp.SmtpClient();

/*            // Для теста можно временно отключить проверку сертификата (НЕ для продакшена)
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;*/

            var smtpServer = emailSettings["SmtpServer"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"]);

            SecureSocketOptions socketOptions = smtpPort switch
            {
                465 => SecureSocketOptions.SslOnConnect,
                587 => SecureSocketOptions.StartTls,
                _ => SecureSocketOptions.Auto
            };

            await client.ConnectAsync(smtpServer, smtpPort, socketOptions);
            await client.AuthenticateAsync(emailSettings["SmtpUser"], emailSettings["SmtpPass"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }

}
