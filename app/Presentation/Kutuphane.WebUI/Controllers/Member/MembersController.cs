using Kutuphane.Application.Dtos.AuthDtos;
using Kutuphane.Application.Dtos.MemberDtos;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.WebUI.Models.ViewModels; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kutuphane.WebUI.Controllers.Member
{
    [Authorize]
    public class MembersController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMemberService _memberService;
        private readonly ILoanService _loanService;
        private readonly IContactService _contactService;

        public MembersController(IAuthService authService, IMemberService memberService, ILoanService loanService, IContactService contactService)
        {
            _authService = authService;
            _memberService = memberService;
            _loanService = loanService;
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null) return NotFound();

     
            var userMessages = await _contactService.GetMessagesByEmailAsync(user.Email);
            ViewBag.UserMessages = userMessages.Take(5).ToList();

            if (user.MemberId.HasValue)
            {
                var member = await _memberService.GetMemberByIdAsync(user.MemberId.Value);
                return View(member);
            }

            TempData["Error"] = "Üye kaydı bulunamadı.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null || !user.MemberId.HasValue)
                return RedirectToAction("Index", "Home");

            var member = await _memberService.GetMemberByIdAsync(user.MemberId.Value);

            var dto = new MemberProfileDto
            {
                Id = member.Id,
                Username = user.Username,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                Phone = member.Phone,
                Address = member.Address,
                DateOfBirth = member.DateOfBirth
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(MemberProfileDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = await _authService.GetUserByIdAsync(userId);

                if (user.MemberId != model.Id)
                {
                    TempData["Error"] = "Hatalı işlem.";
                    return RedirectToAction("Profile");
                }

                await _memberService.UpdateMemberProfileAsync(model);

                TempData["Success"] = "Profil bilgileriniz güncellendi.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

 
        [HttpGet]
        public async Task<IActionResult> MyLoans()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null || !user.MemberId.HasValue)
            {
                TempData["Error"] = "Üye bilgileriniz bulunamadı.";
                return RedirectToAction("Profile");
            }

            var activeLoans = await _loanService.GetMemberActiveLoansAsync(user.MemberId.Value);
            var loanHistory = await _loanService.GetLoanHistoryAsync(user.MemberId.Value, 1, 10);

            ViewBag.ActiveLoans = activeLoans;
            ViewBag.LoanHistory = loanHistory;

            return View();
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _authService.ChangePasswordAsync(userId, model);

                TempData["Success"] = "Şifreniz başarıyla değiştirildi.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

    
        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            try
            {
             
                await _contactService.DeleteMessageAsync(id);
                TempData["Success"] = "Mesaj silindi.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Mesaj silinirken hata oluştu.";
            }
            return RedirectToAction("Profile");
        }
    }
}