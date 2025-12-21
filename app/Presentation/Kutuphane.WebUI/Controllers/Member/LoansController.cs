using Kutuphane.Application.Dtos.LoanDtos;
using Kutuphane.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kutuphane.WebUI.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IAuthService _authService;

        public LoansController(ILoanService loanService, IAuthService authService)
        {
            _loanService = loanService;
            _authService = authService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLoan(int bookId)
        {
            try
            {
  
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return RedirectToAction("Login", "Account");

                int userId = int.Parse(userIdClaim.Value);

                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null || user.MemberId == null)
                {
                    TempData["Error"] = "Ödünç alma işlemi için üye kaydınız bulunamadı.";
                    return RedirectToAction("Details", "Books", new { id = bookId });
                }

            
                var createLoanDto = new CreateLoanDto
                {
                    BookId = bookId,
                    MemberId = user.MemberId.Value,
                    LoanedByUserId = userId
                };

              
                await _loanService.BorrowBookAsync(createLoanDto);

                TempData["Success"] = "Kitap başarıyla ödünç alındı!";
                return RedirectToAction("MyLoans", "Members"); 
            }
            catch (InvalidOperationException ex) 
            {
             
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", "Books", new { id = bookId });
            }
            catch (Exception ex) 
            {
                TempData["Error"] = "Bir hata oluştu: " + ex.Message;
                return RedirectToAction("Details", "Books", new { id = bookId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnLoan(int loanId)
        {
            try
            {
             
                var returnDto = new ReturnLoanDto
                {        
                    Notes = "Kullanıcı tarafından web arayüzü üzerinden iade edildi."
                };

                await _loanService.ReturnBookAsync(loanId, returnDto);

                TempData["Success"] = "Kitap başarıyla iade edildi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İade işlemi başarısız: " + ex.Message;
            }

            return RedirectToAction("MyLoans", "Members");
        }
    }
}