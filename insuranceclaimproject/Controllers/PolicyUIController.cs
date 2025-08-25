using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace insuranceclaimproject.Controllers
{
    [Route("policy")]
 
    public class PolicyUIController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
