using Kutuphane.Application.Dtos.BookDtos;
using Kutuphane.Application.Dtos.ContactDtos;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Application.Services;
using Kutuphane.WebUI.Models;
using Kutuphane.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Kutuphane.WebUI.Controllers
{
    public class HomeController : Controller
    {

        private readonly IBookService _bookService;
        private readonly IReportService _reportService;
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        private readonly IMemberService _memberService;
        private readonly ILoanService _loanService;

        public HomeController(
            IBookService bookService,
            IReportService reportService,
            IUserService userService,
            IContactService contactService,
            IMemberService memberService,
            ILoanService loanService)

        {

            _bookService = bookService;
            _reportService = reportService;
            _userService = userService;
            _contactService = contactService;
            _memberService = memberService;
            _loanService = loanService;
        }

        public async Task<IActionResult> Index()
        {

            var allBooks = await _bookService.GetAllBooksAsync();
            var newBooks = allBooks.OrderByDescending(b => b.Id).Take(8).ToList();


            var popularBookDtos = await _reportService.GetMostBorrowedBooksAsync(5);
            var popularBooks = allBooks
                .Where(b => popularBookDtos.Any(p => p.BookId == b.Id))
                .ToList();


            var categories = allBooks
                .GroupBy(b => b.Category)
                .Select(g => new CategoryStatViewModel
                {
                    CategoryName = g.Key,
                    BookCount = g.Count(),
                    IconCssClass = GetIconForCategory(g.Key)
                })
                .OrderByDescending(c => c.BookCount)
                .Take(4)
                .ToList();


            var model = new HomeIndexViewModel
            {
                NewBooks = newBooks,
                PopularBooks = popularBooks,
                Categories = categories
            };

            return View(model);
        }

        private string GetIconForCategory(string category)
        {
            if (string.IsNullOrEmpty(category)) return "bi-bookmark";

            return category.ToLower() switch
            {
                "roman" => "bi-book-half",
                "bilim" => "bi-lightbulb",
                "tarih" => "bi-clock-history",
                "felsefe" => "bi-mortarboard",
                "çocuk" => "bi-balloon",
                "teknoloji" => "bi-laptop",
                "edebiyat" => "bi-feather",
                _ => "bi-bookmark"
            };
        }



        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            // ViewModel yerine DTO kullanýyoruz
            var model = new ContactMessageDto();

            if (User.Identity.IsAuthenticated)
            {
                var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdString, out int userId))
                {
                    var user = await _userService.GetUserByIdAsync(userId);
                    if (user != null)
                    {
                        model.Name = user.FullName;
                        model.Email = user.Email;
                    }
                }
            }
            return View(model);
        }

     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMessageDto model) // Parametre deðiþti
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                int? userId = null;
                if (User.Identity.IsAuthenticated)
                {
                    var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdString, out int id)) userId = id;
                }

                // Servis artýk DTO kabul ediyor, sorunsuz çalýþýr
                await _contactService.SendMessageAsync(model, userId);

                TempData["Success"] = "Mesajýnýz baþarýyla iletildi. En kýsa sürede size dönüþ yapacaðýz.";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Bir hata oluþtu: " + ex.Message;
                return View(model);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult NotFound(int code)
        {
            return View("~/Views/Shared/NotFound.cshtml");
        }
      
        public async Task<IActionResult> About()
        {
           

            var allBooks = await _bookService.GetAllBooksAsync();
            var allMembers = await _memberService.GetAllMembersAsync();

            var allLoans = await _loanService.GetAllLoansAsync();
            var activeLoansCount = allLoans.Count(l => l.ReturnDate == null); 

          
            var model = new AboutViewModel
            {
                TotalBooks = allBooks.Count(),
                TotalMembers = allMembers.Count(),
                ActiveLoans = activeLoansCount,

                TeamMembers = new List<TeamMemberViewModel>
                {
                    new TeamMemberViewModel
                    {
                        Name = "Erkan Özgün",
                        Title = "Lead Developer / Computer Eng. Student",
                        ImageUrl = "" 
                    },
                
                    new TeamMemberViewModel
                    {
                        Name = "Yusuflar",
                        Title = "Akademik Danýþman",
                        ImageUrl = ""
                    }
                }
            };

            return View(model);
        }
    }
}