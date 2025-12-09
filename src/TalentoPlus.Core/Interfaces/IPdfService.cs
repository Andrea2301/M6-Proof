using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateResumeAsync(Entities.Employee employee);
    }
}
