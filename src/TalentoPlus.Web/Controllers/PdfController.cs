using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Web.Controllers
{
    public class PdfController : Controller
    {
        private readonly IPdfService _pdfService;
        private readonly IUnitOfWork _unitOfWork;

        public PdfController(IPdfService pdfService, IUnitOfWork unitOfWork)
        {
            _pdfService = pdfService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> GenerateResume(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null) return NotFound();
            
            var pdfBytes = await _pdfService.GenerateResumeAsync(employee);
            return File(pdfBytes, "application/pdf", $"Resume_{employee.Document}.pdf");
        }
    }
}
