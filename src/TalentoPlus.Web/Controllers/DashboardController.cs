using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAIService _aiService;

        public DashboardController(IUnitOfWork unitOfWork, IAIService aiService)
        {
            _unitOfWork = unitOfWork;
            _aiService = aiService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalEmployees = await _unitOfWork.Employees.CountAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AskAI([FromBody] string question)
        {
            var answer = await _aiService.ProcessQueryAsync(question);
            return Json(new { answer });
        }
    }
}
