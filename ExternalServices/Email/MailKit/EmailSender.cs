using GalliumPlus.WebApi.Core.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;

namespace GalliumPlus.WebApi.Email.MailKit
{
    /// <summary>
    /// Envoi de mail basé sur MailKit.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private MailKitOptions options;

        public EmailSender(MailKitOptions options)
        {
            this.options = options;
        }

        private MimeMessage BuildMessage(string recipient, string subject, string content)
        {
            MimeMessage result = new MimeMessage();

            result.From.Add(new MailboxAddress(this.options.SenderDisplayName, this.options.SenderAddress));
            result.To.Add(new MailboxAddress("", recipient));

            result.Subject = subject;

            result.Body = new TextPart(TextFormat.Html) { Text = content };

            return result;
        }

        public void Send(string recipient, string subject, string content)
        {
            try
            {
                var message = this.BuildMessage(recipient, subject, content);

                SecureSocketOptions sslOpts = SecureSocketOptions.Auto;
                if (this.options.UseSSL)
                {
                    sslOpts = SecureSocketOptions.SslOnConnect;
                }
                else if (this.options.UseStartTls)
                {
                    sslOpts = SecureSocketOptions.StartTls;
                }

                using var smtp = new SmtpClient();

                smtp.Connect(this.options.Host, this.options.Port, sslOpts);
                smtp.Authenticate(this.options.Username, this.options.Password);

                smtp.Send(message);

                smtp.Disconnect(true);
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        public async Task SendAsync(string recipient, string subject, string content, CancellationToken ct = default)
        {
            try
            {
                var message = this.BuildMessage(recipient, subject, content);

                SecureSocketOptions sslOpts = SecureSocketOptions.Auto;
                if (this.options.UseSSL)
                {
                    sslOpts = SecureSocketOptions.SslOnConnect;
                }
                else if (this.options.UseStartTls)
                {
                    sslOpts = SecureSocketOptions.StartTls;
                }

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(this.options.Host, this.options.Port, sslOpts, ct);
                await smtp.AuthenticateAsync(this.options.Username, this.options.Password, ct);

                await smtp.SendAsync(message, ct);

                await smtp.DisconnectAsync(true, ct);
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }
    }
}