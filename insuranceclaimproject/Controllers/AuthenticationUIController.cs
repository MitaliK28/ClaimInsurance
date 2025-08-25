using Microsoft.AspNetCore.Mvc;

namespace insuranceclaimproject.Controllers
{
    public class AuthenticationUIController : Controller
    {
        [HttpGet]
        public IActionResult LoginRegister()
        {
            return View(); // This looks for Views/AuthenticationUI/LoginRegister.cshtml
        }
    }
}
