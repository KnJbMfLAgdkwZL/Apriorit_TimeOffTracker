using System.Net.Mail;
using System.Threading.Tasks;

namespace TimeOffTracker.Services.EmailService
{
    public interface IEmailService
    {
        public Task SendEmail(MailAddress to, string subject, string html);
    }
}