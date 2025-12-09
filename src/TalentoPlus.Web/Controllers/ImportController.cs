using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TalentoPlus.Core.Interfaces;
using System.IO;
using System.Linq;
using System;

namespace TalentoPlus.Web.Controllers
{
    public class ImportController : Controller
    {
        private readonly IImportService _importService;

        public ImportController(IImportService importService)
        {
            _importService = importService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string fileType)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file.";
                return RedirectToAction("Index");
            }

            // Validate file type
            var allowedTypes = new[] { "xlsx", "xls", "csv" };
            if (!allowedTypes.Contains(fileType.ToLower()))
            {
                TempData["Error"] = "Invalid file type. Please select Excel or CSV.";
                return RedirectToAction("Index");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var result = await _importService.ImportEmployeesAsync(stream, fileType);
                    
                    if (result.Success)
                    {
                        TempData["Success"] = $"Successfully imported {result.ImportedCount} employees.";
                        if (result.FailedCount > 0)
                        {
                            TempData["Warning"] = $"{result.FailedCount} records failed to import.";
                        }
                    }
                    else
                    {
                        TempData["Error"] = $"Import failed: {string.Join(", ", result.Errors.Take(3))}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error processing file: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        public IActionResult DownloadTemplate(string type = "xlsx")
        {
            var fileName = type.ToLower() == "csv" 
                ? "employees-template.csv" 
                : "employees-template.xlsx";
                
            var filePath = Path.Combine("wwwroot", "sample", fileName);
            
            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "Template file not found.";
                return RedirectToAction("Index");
            }

            var mimeType = type.ToLower() == "csv" 
                ? "text/csv" 
                : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                
            return PhysicalFile(filePath, mimeType, fileName);
        }
    }
}
