using Microsoft.AspNetCore.Mvc;

namespace asp.net.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
