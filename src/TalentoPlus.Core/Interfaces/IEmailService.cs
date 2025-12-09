using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
