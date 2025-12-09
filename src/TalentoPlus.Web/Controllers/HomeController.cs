using Microsoft.AspNetCore.Mvc;

namespace TalentoPlus.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
