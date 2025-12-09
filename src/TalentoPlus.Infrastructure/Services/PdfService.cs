using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Threading.Tasks;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        public async Task<byte[]> GenerateResumeAsync(Employee employee)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    
                    page.Header().Text("Employee Resume - TalentoPlus").Bold().FontSize(20);
                    
                    page.Content().Column(column =>
                    {
                        column.Item().Text($"Name: {employee.FirstName} {employee.LastName}");
                        column.Item().Text($"Document: {employee.Document}");
                        column.Item().Text($"Email: {employee.Email}");
                        column.Item().Text($"Salary: ${employee.Salary:N0}");
                    });
                });
            });
            
            return document.GeneratePdf();
        }
    }
}
