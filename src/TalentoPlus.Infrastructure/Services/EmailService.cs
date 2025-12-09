using System.Threading.Tasks;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Simulación para pruebas
            Console.WriteLine($"Email to: {toEmail}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
            await Task.CompletedTask;
        }
    }
}
