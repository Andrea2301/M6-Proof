using Microsoft.AspNetCore.Http;
using TalentoPlus.Infrastructure.Interfaces;

namespace TalentoPlus.Infrastructure.Interfaces
{
    public interface IExcelImportService
    {
        Task<ExcelImportResult> ImportEmployeesAsync(IFormFile file);
    }

    public class ExcelImportResult
    {
        public bool Success { get; set; }
        public int TotalRows { get; set; }
        public int SuccessfulImports { get; set; }
        public int UpdatedRecords { get; set; }
        public int FailedImports { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}