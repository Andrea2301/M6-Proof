using System.Threading.Tasks;
using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateEmployeeResume(Employee employee);
        Task<byte[]> GenerateEmployeeResumeAsync(int employeeId);
    }
}
