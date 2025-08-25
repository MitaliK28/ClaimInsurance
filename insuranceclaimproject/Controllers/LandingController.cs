using Microsoft.AspNetCore.Mvc;

namespace insuranceclaimproject.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
