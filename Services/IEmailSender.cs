using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string httpMessage);
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailsettings;

        public EmailSender(IOptions<EmailSettings> emailsettings)
        {
            _emailsettings = emailsettings.Value;
        }


        public async Task SendEmailAsync(string email, string subject, string httpMessage)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailsettings.SenderName, _emailsettings.SenderEmail));
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            mimeMessage.Subject = subject;
            var builder = new BodyBuilder { HtmlBody = httpMessage };
            mimeMessage.Body = builder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(_emailsettings.MailServer, _emailsettings.MailPort, _emailsettings.UseSsl)
                            .ConfigureAwait(false);
                await client.AuthenticateAsync(_emailsettings.SenderEmail, _emailsettings.Password).ConfigureAwait(false);
                await client.SendAsync(mimeMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
