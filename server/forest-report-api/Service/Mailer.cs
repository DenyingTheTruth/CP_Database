using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace forest_report_api.Service
{
    public class Mailer : IMailer
    {
        public async Task SendEmailAsync(string email, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Forest", "pashashichko0075@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("name",email));
            emailMessage.Subject = "Forest";

            var builder = new BodyBuilder {HtmlBody = message};

            emailMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync("smtp.gmail.com", 587,
                    MailKit.Security.SecureSocketOptions.StartTls);

                await client.AuthenticateAsync("pashashichko0075@gmail.com", "Puzahagoogle");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}