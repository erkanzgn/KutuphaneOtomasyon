using Microsoft.AspNetCore.Mvc;

namespace Kutuphane.WebUI.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
