using Kutuphane.Application.Interfaces.Services;
using Kutuphane.WebUI.Models;
using Kutuphane.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Kutuphane.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;
        public HomeController(ILogger<HomeController> logger, IBookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            var recentBook = await _bookService.GetAllBooksAsync();
            var model = recentBook.OrderByDescending(x => x.Id).Take(4);
            return View(model);
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO: Email gönderme iþlemi (opsiyonel)

            TempData["Success"] = "Mesajýnýz baþarýyla gönderildi. En kýsa sürede size dönüþ yapacaðýz.";
            return RedirectToAction("Contact");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
