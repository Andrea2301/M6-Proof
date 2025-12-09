using System.Net.Http.Json;
using TalentoPlus.Core.DTOs;

namespace TalentoPlus.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        #region Generic Helpers

        private async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>(url);
            }
            catch
            {
                return default;
            }
        }

        private async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, data);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch { }
            return default;
        }

        private async Task<bool> PutAsync<TRequest>(string url, TRequest data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(url, data);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        private async Task<bool> DeleteAsync(string url)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        #endregion

        #region Departments

        public Task<List<DepartmentDto>> GetDepartmentsAsync() =>
            GetAsync<List<DepartmentDto>>("api/Departments") ?? Task.FromResult(new List<DepartmentDto>());

        public Task<DepartmentDto> GetDepartmentByIdAsync(int id) =>
            GetAsync<DepartmentDto>($"api/Departments/{id}") ?? Task.FromResult<DepartmentDto>(null!);

        #endregion

        #region Employees

        public Task<List<EmployeeResponseDto>> GetEmployeesAsync() =>
            GetAsync<List<EmployeeResponseDto>>("api/Employees") ?? Task.FromResult(new List<EmployeeResponseDto>());

        public Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id) =>
            GetAsync<EmployeeResponseDto>($"api/Employees/{id}") ?? Task.FromResult<EmployeeResponseDto>(null!);

        public Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto employee) =>
            PostAsync<CreateEmployeeDto, EmployeeResponseDto>("api/Employees", employee)!;

        public Task UpdateEmployeeAsync(int id, UpdateEmployeeDto employee) =>
            PutAsync($"api/Employees/{id}", employee);

        public Task DeleteEmployeeAsync(int id) =>
            DeleteAsync($"api/Employees/{id}");

        #endregion

        #region Positions

        public Task<List<PositionDto>> GetPositionsAsync() =>
            GetAsync<List<PositionDto>>("api/Positions") ?? Task.FromResult(new List<PositionDto>());

        #endregion

        #region EducationLevels

        public Task<List<EducationLevelDto>> GetEducationLevelsAsync() =>
            GetAsync<List<EducationLevelDto>>("api/EducationLevels") ?? Task.FromResult(new List<EducationLevelDto>());

        #endregion

        #region EmployeeStatuses

        public Task<List<EmployeeStatusDto>> GetEmployeeStatusesAsync() =>
            GetAsync<List<EmployeeStatusDto>>("api/EmployeeStatuses") ?? Task.FromResult(new List<EmployeeStatusDto>());

        #endregion

        #region Auth

        public async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", new { email, password });
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
            }
            catch { }
            return string.Empty;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/Register", registerDto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        #endregion
    }
}
