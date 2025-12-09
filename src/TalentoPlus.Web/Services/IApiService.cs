using TalentoPlus.Core.DTOs;

namespace TalentoPlus.Web.Services
{
    public interface IApiService
    {
        // Departments
        Task<List<DepartmentDto>> GetDepartmentsAsync();
        Task<DepartmentDto> GetDepartmentByIdAsync(int id);
        
        // Employees
        Task<List<EmployeeResponseDto>> GetEmployeesAsync();
        Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto employee);
        Task UpdateEmployeeAsync(int id, UpdateEmployeeDto employee);
        Task DeleteEmployeeAsync(int id);
        
        // Positions
        Task<List<PositionDto>> GetPositionsAsync();
        
        // EducationLevels
        Task<List<EducationLevelDto>> GetEducationLevelsAsync();
        
        // EmployeeStatuses
        Task<List<EmployeeStatusDto>> GetEmployeeStatusesAsync();
        
        // Auth (si tu API tiene endpoints de auth)
        Task<string> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(RegisterDto registerDto);
    }
}