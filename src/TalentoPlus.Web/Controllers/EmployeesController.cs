using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Web.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return View(employees);
        }
    }
}
