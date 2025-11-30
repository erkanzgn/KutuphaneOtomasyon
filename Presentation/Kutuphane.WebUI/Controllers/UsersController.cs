using Microsoft.AspNetCore.Mvc;

namespace Kutuphane.WebUI.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
