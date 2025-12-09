using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Infrastructure.Interfaces;

namespace TalentoPlus.Infrastructure.Services
{
    public class ExcelImportService : IExcelImportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExcelImportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExcelImportResult> ImportEmployeesAsync(IFormFile file)
        {
            var result = new ExcelImportResult();

            try
            {
                // Validar archivo
                if (file == null || file.Length == 0)
                {
                    result.Success = false;
                    result.Message = "No se proporcionó ningún archivo";
                    return result;
                }

                if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                {
                    result.Success = false;
                    result.Message = "El archivo debe ser un Excel (.xlsx o .xls)";
                    return result;
                }

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();

                // Obtener catálogos existentes
                var departments = await _unitOfWork.Departments.GetAllAsync();
                var positions = await _unitOfWork.Positions.GetAllAsync();
                var educationLevels = await _unitOfWork.EducationLevels.GetAllAsync();
                var statuses = await _unitOfWork.EmployeeStatuses.GetAllAsync();

                var departmentDict = departments.ToDictionary(d => d.Name.Trim().ToLower(), d => d);
                var positionDict = positions.ToDictionary(p => p.Name.Trim().ToLower(), p => p);
                var educationDict = educationLevels.ToDictionary(e => e.Name.Trim().ToLower(), e => e);
                var statusDict = statuses.ToDictionary(s => s.Name.Trim().ToLower(), s => s);

                var rows = worksheet.RowsUsed().Skip(1); // Skip header
                result.TotalRows = rows.Count();

                foreach (var row in rows)
                {
                    try
                    {
                        // Columna 1: Documento
                        var document = row.Cell(1).GetString().Trim();
                        
                        if (string.IsNullOrEmpty(document))
                        {
                            result.FailedImports++;
                            result.Errors.Add($"Fila {row.RowNumber()}: Documento vacío");
                            continue;
                        }

                        // Buscar si el empleado ya existe
                        var existingEmployee = await _unitOfWork.Employees.GetByDocumentAsync(document);
                        var employee = existingEmployee ?? new Employee();
                        var isNewEmployee = existingEmployee == null;

                        // Column 2: Nombres
                        employee.FirstName = row.Cell(2).GetString().Trim();
                        if (string.IsNullOrEmpty(employee.FirstName))
                        {
                            result.FailedImports++;
                            result.Errors.Add($"Fila {row.RowNumber()}: Nombre vacío");
                            continue;
                        }

                        // Columna 3: Apellidos
                        employee.LastName = row.Cell(3).GetString().Trim();
                        if (string.IsNullOrEmpty(employee.LastName))
                        {
                            result.FailedImports++;
                            result.Errors.Add($"Fila {row.RowNumber()}: Apellido vacío");
                            continue;
                        }

                        employee.Document = document;

                        // Columna 4: FechaNacimiento
                        try
                        {
                            var birthDateCell = row.Cell(4);
                            if (!birthDateCell.IsEmpty())
                            {
                                if (birthDateCell.TryGetValue(out DateTime birthDate))
                                {
                                    employee.BirthDate = birthDate;
                                }
                                else if (DateTime.TryParse(birthDateCell.GetString(), out birthDate))
                                {
                                    employee.BirthDate = birthDate;
                                }
                            }
                        }
                        catch
                        {
                            result.Warnings.Add($"Fila {row.RowNumber()}: Fecha de nacimiento inválida, se omitió");
                        }

                        // Columna 5: Direccion
                        employee.Address = row.Cell(5).GetString().Trim();

                        // Columna 6: Telefono
                        employee.Phone = row.Cell(6).GetString().Trim();

                        // Columna 7: Email
                        employee.Email = row.Cell(7).GetString().Trim();
                        if (string.IsNullOrEmpty(employee.Email))
                        {
                            result.FailedImports++;
                            result.Errors.Add($"Fila {row.RowNumber()}: Email vacío");
                            continue;
                        }

                        // Columna 8: Cargo (Position)
                        var cargoName = row.Cell(8).GetString().Trim();
                        if (string.IsNullOrEmpty(cargoName))
                        {
                            result.FailedImports++;
                            result.Errors.Add($"Fila {row.RowNumber()}: Cargo vacío");
                            continue;
                        }

                        var cargoLower = cargoName.ToLower();
                        if (positionDict.TryGetValue(cargoLower, out var position))
                        {
                            employee.PositionId = position.Id;
                        }
                        else
                        {
                            var newPosition = new Position { Name = cargoName };
                            await _unitOfWork.Positions.AddAsync(newPosition);
                            await _unitOfWork.CompleteAsync();
                            employee.PositionId = newPosition.Id;
                            positionDict[cargoLower] = newPosition;
                            result.Warnings.Add($"Fila {row.RowNumber()}: Se creó nuevo cargo '{cargoName}'");
                        }

                        // Columna 9: Salario
                        try
                        {
                            var salaryCell = row.Cell(9);
                            if (!salaryCell.IsEmpty())
                            {
                                if (salaryCell.TryGetValue(out double salaryDouble))
                                {
                                    employee.Salary = (decimal)salaryDouble;
                                }
                                else if (decimal.TryParse(salaryCell.GetString().Replace("$", "").Replace(",", ""), out var salary))
                                {
                                    employee.Salary = salary;
                                }
                            }
                        }
                        catch
                        {
                            result.Warnings.Add($"Fila {row.RowNumber()}: Salario inválido, se asignó 0");
                            employee.Salary = 0;
                        }

                        // Columna 10: FechaIngreso
                        try
                        {
                            var hireDateCell = row.Cell(10);
                            if (!hireDateCell.IsEmpty())
                            {
                                if (hireDateCell.TryGetValue(out DateTime hireDate))
                                {
                                    employee.HireDate = hireDate;
                                }
                                else if (DateTime.TryParse(hireDateCell.GetString(), out hireDate))
                                {
                                    employee.HireDate = hireDate;
                                }
                                else
                                {
                                    employee.HireDate = DateTime.UtcNow;
                                }
                            }
                            else
                            {
                                employee.HireDate = DateTime.UtcNow;
                            }
                        }
                        catch
                        {
                            employee.HireDate = DateTime.UtcNow;
                            result.Warnings.Add($"Fila {row.RowNumber()}: Fecha de ingreso inválida, se usó fecha actual");
                        }

                        // Columna 11: Estado
                        var estadoName = row.Cell(11).GetString().Trim();
                        if (string.IsNullOrEmpty(estadoName))
                        {
                            result.FailedImports++;
                            result.Errors.Add($"Fila {row.RowNumber()}: Estado vacío");
                            continue;
                        }

                        var estadoLower = estadoName.ToLower();
                        // Mapear estados comunes en español a inglés
                        var estadoMapping = new Dictionary<string, string>
                        {
                            { "activo", "active" },
                            { "inactivo", "inactive" },
                            { "vacaciones", "vacations" }
                        };

                        var estadoKey = estadoMapping.ContainsKey(estadoLower) 
                            ? estadoMapping[estadoLower] 
                            : estadoLower;

                        if (statusDict.TryGetValue(estadoKey, out var status))
                        {
                            employee.EmployeeStatusId = status.Id;
                        }
                        else
                        {
                            result.FailedImports++;
                            result.Errors.Add($"Fila {row.RowNumber()}: Estado '{estadoName}' no válido. Estados permitidos: Activo, Inactivo, Vacaciones");
                            continue;
                        }

                        // Columna 12: NivelEducativo
                        var educacionName = row.Cell(12).GetString().Trim();
                        if (string.IsNullOrEmpty(educacionName))
                        {
                            result.FailedImports++;
                            result.Errors.Add($"Fila {row.RowNumber()}: Nivel educativo vacío");
                            continue;
                        }

                        var educacionLower = educacionName.ToLower();
                        if (educationDict.TryGetValue(educacionLower, out var education))
                        {
                            employee.EducationLevelId = education.Id;
                        }
                        else
                        {
                            var newEducation = new EducationLevel { Name = educacionName };
                            await _unitOfWork.EducationLevels.AddAsync(newEducation);
                            await _unitOfWork.CompleteAsync();
                            employee.EducationLevelId = newEducation.Id;
                            educationDict[educacionLower] = newEducation;
                            result.Warnings.Add($"Fila {row.RowNumber()}: Se creó nuevo nivel educativo '{educacionName}'");
                        }

                        // Columna 13: PerfilProfesional
                        employee.ProfessionalProfile = row.Cell(13).GetString().Trim();

                        // Columna 14: Departamento
                        var deptName = row.Cell(14).GetString().Trim();
                        if (string.IsNullOrEmpty(deptName))
                        {
                            result.FailedImports++;
                            result.Errors.Add($"Fila {row.RowNumber()}: Departamento vacío");
                            continue;
                        }

                        var deptLower = deptName.ToLower();
                        if (departmentDict.TryGetValue(deptLower, out var dept))
                        {
                            employee.DepartmentId = dept.Id;
                        }
                        else
                        {
                            var newDept = new Department { Name = deptName };
                            await _unitOfWork.Departments.AddAsync(newDept);
                            await _unitOfWork.CompleteAsync();
                            employee.DepartmentId = newDept.Id;
                            departmentDict[deptLower] = newDept;
                            result.Warnings.Add($"Fila {row.RowNumber()}: Se creó nuevo departamento '{deptName}'");
                        }

                        // Guardar empleado
                        if (isNewEmployee)
                        {
                            employee.RegistrationDate = DateTime.UtcNow;
                            employee.IsEnabled = false;
                            await _unitOfWork.Employees.AddAsync(employee);
                            result.SuccessfulImports++;
                        }
                        else
                        {
                            _unitOfWork.Employees.Update(employee);
                            result.UpdatedRecords++;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.FailedImports++;
                        result.Errors.Add($"Fila {row.RowNumber()}: {ex.Message}");
                    }
                }

                await _unitOfWork.CompleteAsync();

                result.Success = result.FailedImports == 0 || result.SuccessfulImports > 0;
                result.Message = $"Importación completada: {result.SuccessfulImports} nuevos, {result.UpdatedRecords} actualizados, {result.FailedImports} fallidos";

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error general: {ex.Message}";
                result.Errors.Add(ex.ToString());
                return result;
            }
        }
    }
}