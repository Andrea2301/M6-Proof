using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        public async Task<bool> ValidateExcelStructureAsync(Stream fileStream)
        {
            try
            {
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets[0];
                return worksheet.Dimension.Rows > 1;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<IEnumerable<Employee>> ReadEmployeesFromExcelAsync(Stream fileStream)
        {
            var employees = new List<Employee>();
            
            // Simulación para pruebas
            employees.Add(new Employee
            {
                Document = "1001",
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                Salary = 50000,
                BirthDate = DateTime.Now.AddYears(-30),
                HireDate = DateTime.Now,
                ProfessionalProfile = "Test profile"
            });
            
            return employees;
        }
    }
}
