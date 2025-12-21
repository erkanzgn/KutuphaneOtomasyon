using Kutuphane.Application.Dtos.LoanDtos;
using Kutuphane.Application.Exceptions;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Kutuphane.WebUI.Controllers.Admin
{
    [Authorize(Roles = "Admin,Librarian")] 
    public class AdminLoansController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;  
        private readonly IMemberService _memberService; 

        public AdminLoansController(ILoanService loanService, IBookService bookService, IMemberService memberService)
        {
            _loanService = loanService;
            _bookService = bookService;
            _memberService = memberService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            // 1. Tüm veriyi çek
            var loans = await _loanService.GetAllLoansAsync();

            // 2. Arama terimi varsa filtrele
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();

                loans = loans.Where(x =>
                    (x.MemberName != null && x.MemberName.ToLower().Contains(searchTerm)) || 
                    (x.BookTitle != null && x.BookTitle.ToLower().Contains(searchTerm)) ||   
                    x.MemberId.ToString().Contains(searchTerm) // 
                ).ToList();
            }

          
            ViewBag.SearchTerm = searchTerm;

            return View(loans);
        }


        [HttpGet]
        public async Task<IActionResult> Overdue()
        {
            var overdueLoans = await _loanService.GetOverdueLoansAsync();
            return View("Index", overdueLoans); 
        }

      
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareDropdowns();
                return View(model);
            }

            try
            {
             
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int currentUserId = int.TryParse(userIdString, out var id) ? id : 0;

                var createDto = new CreateLoanDto
                {
                    MemberId = model.MemberId,
                    CopyId = model.IsAutoSelection ? 0 : model.CopyId ?? 0,
                    BookId = model.BookId ?? 0,
                    LoanedByUserId = currentUserId, 
                    Notes = model.Notes
                };

                await _loanService.BorrowBookAsync(createDto);

                TempData["Success"] = "Kitap başarıyla ödünç verildi.";
                return RedirectToAction("Index");
            }
  
            catch (MemberNotEligibleException ex)
            {
                ModelState.AddModelError("", "Engel: " + ex.Message); // Örn: 2 kitap limiti, gecikmiş kitap
            }
            catch (CopyNotAvailableException ex)
            {
                ModelState.AddModelError("", "Hata: Seçilen kopya şu an müsait değil.");
            }
            catch (NotFoundException ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message} bulunamadı.");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", "Stok Hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmeyen hata: " + ex.Message);
            }

            // Hata durumunda dropdownları tekrar doldur
            await PrepareDropdowns();
            return View(model);
        }

        // 5. İADE ALMA İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(int id, string? returnNotes)
        {
            try
            {
                var dto = new ReturnLoanDto { Notes = returnNotes };
                await _loanService.ReturnBookAsync(id, dto);

                TempData["Success"] = "Kitap başarıyla iade alındı.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İade alınırken hata: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

  
        private async Task PrepareDropdowns()
        {
            

            var members = await _memberService.GetAllMembersAsync();
            var books = await _bookService.GetAllBooksAsync(); 

         
            ViewBag.Members = new SelectList(members.Where(m => m.Status == "Aktif"), "Id", "FullName");

            ViewBag.Books = new SelectList(books, "Id", "Title");
        }
 
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
           
            var loan = await _loanService.GetLoanByIdAsync(id);

            if (loan == null) return NotFound();

            return View(loan);
        }
    }
}