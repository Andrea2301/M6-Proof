using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces
{
    public interface IImportService : IExcelService  // Hereda de IExcelService
    {
        // CSV Methods
        Task<IEnumerable<Employee>> ReadEmployeesFromCsvAsync(Stream fileStream);
        Task<bool> ValidateCsvStructureAsync(Stream fileStream);
        
        // Common Import Method
        Task<ImportResult> ImportEmployeesAsync(Stream fileStream, string fileType);
    }
    
    public class ImportResult
    {
        public bool Success { get; set; }
        public int ImportedCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<Employee> ImportedEmployees { get; set; } = new();
    }
}
