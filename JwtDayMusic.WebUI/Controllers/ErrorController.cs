using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebUI.Controllers
{
    public class ErrorController : Controller
    {
       public IActionResult Forbidden403()
        {
            return View();
        }
        public IActionResult Unauthorized401()
        {
            return View();
        }
        public IActionResult NotFound404()
        {
            return View();
        }
    }
}
