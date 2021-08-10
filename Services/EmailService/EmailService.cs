using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TimeOffTracker.Model;

namespace TimeOffTracker.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;

        public EmailService(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }

        public async Task SendEmail(MailAddress to, string subject, string html)
        {
            var from = new MailAddress(_emailOptions.SmtpUser);
            var message = new MailMessage(from, to)
            {
                Subject = subject, Body = html, IsBodyHtml = true
            };

            var smptClient = new SmtpClient(_emailOptions.SmtpHost, Convert.ToInt32(_emailOptions.SmtpPort))
            {
                Credentials = new NetworkCredential(_emailOptions.SmtpUser, _emailOptions.SmtpPass), EnableSsl = true
            };

            await smptClient.SendMailAsync(message);
        }
    }
}