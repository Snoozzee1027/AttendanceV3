using Microsoft.AspNetCore.Mvc;

namespace AttendanceV3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Must have a Views/Home/Index.cshtml
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
