using Microsoft.AspNetCore.Mvc;

namespace insuranceclaimproject.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult dashboardforuser()
        {
            return View();
        }

        public IActionResult claimform()
        {
            return View();
        }
        public IActionResult claim()
        {
            return View();
        }

        public IActionResult policyrequest()
        {
            return View();
        }
        public IActionResult supportticket()
        {
            return View();
        }

        public IActionResult usermanagement()
        {
            return View();
        }

    }
}
