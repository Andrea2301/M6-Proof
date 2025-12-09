using System.Threading.Tasks;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services
{
    public class GeminiService : IAIService
    {
        public async Task<string> ProcessQueryAsync(string question)
        {
            return $"You asked: '{question}'. This is a simulated AI response.";
        }
    }
}
