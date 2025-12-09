using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendWelcomeEmailAsync(string toEmail, string employeeName);
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    
    }
}
