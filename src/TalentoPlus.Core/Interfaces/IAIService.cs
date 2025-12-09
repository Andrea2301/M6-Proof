using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IAIService
    {
        Task<string> ProcessQueryAsync(string question);
    }
}
