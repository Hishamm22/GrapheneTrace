using Microsoft.AspNetCore.Mvc;

namespace GrapheneTrace.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
