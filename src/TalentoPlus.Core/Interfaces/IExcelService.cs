using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces
{
    public interface IExcelService
    {
        Task<IEnumerable<Employee>> ReadEmployeesFromExcelAsync(Stream fileStream);
        Task<bool> ValidateExcelStructureAsync(Stream fileStream);
    }
}
