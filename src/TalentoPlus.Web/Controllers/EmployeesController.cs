using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.DTOs;
using TalentoPlus.Web.Services;

namespace TalentoPlus.Web.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IApiService _apiService;

        public EmployeesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Employees (ÚNICA VISTA)
        public async Task<IActionResult> Index()
        {
            var employees = await _apiService.GetEmployeesAsync();
            return View(employees);
        }
    }
}