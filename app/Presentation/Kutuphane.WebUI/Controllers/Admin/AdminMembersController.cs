using Kutuphane.Application.Dtos.MemberDtos;
using Kutuphane.Application.Exceptions;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kutuphane.WebUI.Controllers.Admin
{
    [Authorize(Roles = "Admin,Librarian")]
    public class AdminMembersController : Controller
    {
        private readonly IMemberService _memberService;

        public AdminMembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            var members = await _memberService.SearchMembersAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;

            return View(members);
        }

      
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var memberWithLoans = await _memberService.GetMemberWithLoansAsync(id);

            if (memberWithLoans == null)
            {
                TempData["Error"] = "Üye bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(memberWithLoans);
        }

    
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _memberService.CreateMemberAsync(model);
                TempData["Success"] = "Üye başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            catch (DuplicateException ex)
            {
                ModelState.AddModelError("", ex.Message); 
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Hata: " + ex.Message);
                return View(model);
            }
        }

   
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null) return NotFound();


            var updateDto = new UpdateMemberDto
            {
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                Phone = member.Phone,
                Address = member.Address,
                DateOfBirth = member.DateOfBirth,
                Notes = member.Notes,
                Status = member.Status 
            };

            ViewBag.MemberId = id;

            return View(updateDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateMemberDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MemberId = id;
                return View(model);
            }

            try
            {
                await _memberService.UpdateMemberAsync(id, model);
                TempData["Success"] = "Üye bilgileri güncellendi.";
                return RedirectToAction("Index");
            }
            catch (DuplicateException ex)
            {
                ModelState.AddModelError("", "Bu e-posta veya telefon başka üyede kayıtlı.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Hata: " + ex.Message);
            }

            ViewBag.MemberId = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _memberService.DeleteMemberAsync(id);
                TempData["Success"] = "Üye silindi.";
            }
            catch (BusinessException ex)
            {
             
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Silme hatası: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBan(int id)
        {
            try
            {
                await _memberService.RemoveBanAsync(id);
                TempData["Success"] = "Üyenin üzerindeki tüm engeller ve cezalar kaldırıldı.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İşlem başarısız: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}