using Microsoft.AspNetCore.Mvc;

namespace insuranceclaimproject.Controllers
{
    public class UserDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
