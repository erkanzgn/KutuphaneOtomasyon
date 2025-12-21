using Kutuphane.Application.Interfaces.Services;
using Kutuphane.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane.WebUI.Controllers.Admin
{
    [Authorize(Roles = "Admin,Librarian")]
    public class AdminController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IContactService _contactService;

        public AdminController(IReportService reportService, IContactService contactService)
        {
            _reportService = reportService;
            _contactService = contactService;
        }

        public async Task<IActionResult> Index()
        {
          
            var stats = await _reportService.GetDashboardStatisticsAsync();


            var overdueReport = await _reportService.GetOverdueLoansReportAsync();

    
            var popularBooks = await _reportService.GetMostBorrowedBooksAsync(5);
            var unreadMessages = await _contactService.GetUnreadMessagesAsync();

            var model = new AdminDashboardViewModel
            {
                Stats = stats,
                OverdueLoans = overdueReport.ToList(), 
                PopularBooks = popularBooks.ToList(),
                RecentMessages = unreadMessages.Take(5).ToList(), 
                UnreadMessageCount = unreadMessages.Count()
            };
      
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Reply(int id, string replyMessage)
        {
           
            await _contactService.ReplyToMessageAsync(id, replyMessage);

           
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Stats()
        {

            var model = await _reportService.GetDetailedStatsAsync();

            return View(model);
        }
    }
}
