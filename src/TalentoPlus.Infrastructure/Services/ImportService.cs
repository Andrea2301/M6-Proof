using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services
{
    public class ImportService : IImportService  // Solo IImportService, que ya incluye IExcelService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public ImportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region IExcelService Implementation
        public async Task<IEnumerable<Employee>> ReadEmployeesFromExcelAsync(Stream fileStream)
        {
            var employees = new List<Employee>();
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension?.Rows ?? 0;
                
                if (rowCount <= 1) return employees;
                
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var employee = new Employee
                        {
                            Document = GetCellValue(worksheet, row, 1),
                            FirstName = GetCellValue(worksheet, row, 2),
                            LastName = GetCellValue(worksheet, row, 3),
                            Email = GetCellValue(worksheet, row, 4),
                            Phone = GetCellValue(worksheet, row, 5),
                            Address = GetCellValue(worksheet, row, 6),
                            BirthDate = ParseDate(GetCellValue(worksheet, row, 7)),
                            Salary = ParseDecimal(GetCellValue(worksheet, row, 8)),
                            HireDate = ParseDate(GetCellValue(worksheet, row, 9)),
                            ProfessionalProfile = GetCellValue(worksheet, row, 10),
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Temp123!"),
                            IsEnabled = true,
                            RegistrationDate = DateTime.UtcNow
                        };
                        
                        await SetForeignKeysAsync(employee, worksheet, row);
                        employees.Add(employee);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading row {row}: {ex.Message}");
                    }
                }
            }
            
            return await Task.FromResult(employees);
        }

        public async Task<bool> ValidateExcelStructureAsync(Stream fileStream)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                
                using (var package = new ExcelPackage(fileStream))
                {
                    if (package.Workbook.Worksheets.Count == 0)
                        return false;
                    
                    var worksheet = package.Workbook.Worksheets[0];
                    
                    // Check required columns
                    var requiredHeaders = new[]
                    {
                        "Document", "FirstName", "LastName", "Email"
                    };
                    
                    for (int i = 0; i < requiredHeaders.Length; i++)
                    {
                        var header = worksheet.Cells[1, i + 1].Text?.Trim();
                        if (string.IsNullOrEmpty(header) || !header.Contains(requiredHeaders[i]))
                        {
                            return false;
                        }
                    }
                    
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region CSV Implementation
        public async Task<IEnumerable<Employee>> ReadEmployeesFromCsvAsync(Stream fileStream)
        {
            var employees = new List<Employee>();
            
            using (var reader = new StreamReader(fileStream))
            {
                string line;
                int lineNumber = 0;
                
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    if (lineNumber == 1) continue; // Skip header
                    
                    var values = ParseCsvLine(line);
                    
                    if (values.Length >= 4) // At least basic info
                    {
                        try
                        {
                            var employee = new Employee
                            {
                                Document = values[0],
                                FirstName = values[1],
                                LastName = values[2],
                                Email = values[3],
                                Phone = values.Length > 4 ? values[4] : "",
                                Address = values.Length > 5 ? values[5] : "",
                                BirthDate = values.Length > 6 ? ParseDate(values[6]) : DateTime.Now.AddYears(-30),
                                Salary = values.Length > 7 ? ParseDecimal(values[7]) : 0,
                                HireDate = values.Length > 8 ? ParseDate(values[8]) : DateTime.Now,
                                ProfessionalProfile = values.Length > 9 ? values[9] : "",
                                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Temp123!"),
                                IsEnabled = true,
                                RegistrationDate = DateTime.UtcNow
                            };
                            
                            employees.Add(employee);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing CSV line {lineNumber}: {ex.Message}");
                        }
                    }
                }
            }
            
            return employees;
        }

        public async Task<bool> ValidateCsvStructureAsync(Stream fileStream)
        {
            try
            {
                using (var reader = new StreamReader(fileStream))
                {
                    var headerLine = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(headerLine))
                        return false;
                    
                    var headers = ParseCsvLine(headerLine);
                    
                    // Check for minimum required columns
                    var requiredColumns = new[] { "Document", "FirstName", "LastName", "Email" };
                    
                    for (int i = 0; i < Math.Min(requiredColumns.Length, headers.Length); i++)
                    {
                        if (!headers[i].Contains(requiredColumns[i]))
                            return false;
                    }
                    
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Common Import Method
        public async Task<ImportResult> ImportEmployeesAsync(Stream fileStream, string fileType)
        {
            var result = new ImportResult();
            
            try
            {
                IEnumerable<Employee> employees;
                
                if (fileType.ToLower() == "xlsx" || fileType.ToLower() == "xls")
                {
                    if (!await ValidateExcelStructureAsync(fileStream))
                    {
                        result.Errors.Add("Invalid Excel file structure");
                        return result;
                    }
                    
                    fileStream.Position = 0;
                    employees = await ReadEmployeesFromExcelAsync(fileStream);
                }
                else if (fileType.ToLower() == "csv")
                {
                    if (!await ValidateCsvStructureAsync(fileStream))
                    {
                        result.Errors.Add("Invalid CSV file structure");
                        return result;
                    }
                    
                    fileStream.Position = 0;
                    employees = await ReadEmployeesFromCsvAsync(fileStream);
                }
                else
                {
                    result.Errors.Add($"Unsupported file type: {fileType}");
                    return result;
                }
                
                // Save to database
                foreach (var employee in employees)
                {
                    try
                    {
                        await _unitOfWork.Employees.AddAsync(employee);
                        result.ImportedCount++;
                        result.ImportedEmployees.Add(employee);
                    }
                    catch (Exception ex)
                    {
                        result.FailedCount++;
                        result.Errors.Add($"Error saving employee {employee.Document}: {ex.Message}");
                    }
                }
                
                await _unitOfWork.CompleteAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Import failed: {ex.Message}");
                result.Success = false;
            }
            
            return result;
        }
        #endregion

        #region Helper Methods
        private string GetCellValue(ExcelWorksheet worksheet, int row, int col)
        {
            return worksheet.Cells[row, col].Text?.Trim() ?? "";
        }
        
        private DateTime ParseDate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return DateTime.Now.AddYears(-30);
            
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return result;
            
            if (double.TryParse(value, out double oaDate))
                return DateTime.FromOADate(oaDate);
            
            return DateTime.Now.AddYears(-30);
        }
        
        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            
            if (decimal.TryParse(value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result))
                return result;
            
            return 0;
        }
        
        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var inQuotes = false;
            var current = "";
            
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.Trim());
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            
            result.Add(current.Trim());
            return result.ToArray();
        }
        
        private async Task SetForeignKeysAsync(Employee employee, ExcelWorksheet worksheet, int row)
        {
            // Try to find position by name (column 11)
            var positionName = GetCellValue(worksheet, row, 11);
            if (!string.IsNullOrEmpty(positionName))
            {
                var position = (await _unitOfWork.Positions.FindAsync(p => p.Name == positionName))
                    .FirstOrDefault();
                if (position != null)
                    employee.PositionId = position.Id;
            }
            
            // Try to find department by name (column 12)
            var departmentName = GetCellValue(worksheet, row, 12);
            if (!string.IsNullOrEmpty(departmentName))
            {
                var department = (await _unitOfWork.Departments.FindAsync(d => d.Name == departmentName))
                    .FirstOrDefault();
                if (department != null)
                    employee.DepartmentId = department.Id;
            }
            
            // Default to first if not found
            if (employee.PositionId == 0)
            {
                var firstPosition = (await _unitOfWork.Positions.GetAllAsync()).FirstOrDefault();
                if (firstPosition != null)
                    employee.PositionId = firstPosition.Id;
            }
            
            if (employee.DepartmentId == 0)
            {
                var firstDepartment = (await _unitOfWork.Departments.GetAllAsync()).FirstOrDefault();
                if (firstDepartment != null)
                    employee.DepartmentId = firstDepartment.Id;
            }
            
            // Default status and education
            var firstStatus = (await _unitOfWork.EmployeeStatuses.GetAllAsync()).FirstOrDefault();
            if (firstStatus != null)
                employee.EmployeeStatusId = firstStatus.Id;
                
            var firstEducation = (await _unitOfWork.EducationLevels.GetAllAsync()).FirstOrDefault();
            if (firstEducation != null)
                employee.EducationLevelId = firstEducation.Id;
        }
        #endregion
    }
}
