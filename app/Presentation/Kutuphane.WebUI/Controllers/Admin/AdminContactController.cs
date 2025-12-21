using Kutuphane.Application.Dtos.ContactDtos;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Domain.Enums;
using Kutuphane.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Kutuphane.WebUI.Controllers.Admin
{
    [Authorize(Roles = "Admin,Librarian")]
    public class AdminContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IUserService _userService;
        private readonly IMemberService _memberService;

        public AdminContactController(IContactService contactService, IUserService userService, IMemberService memberService)
        {
            _contactService = contactService;
            _userService = userService;
            _memberService = memberService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var allMessages = await _contactService.GetAllMessagesAsync();

            var model = new ContactListViewModel
            {
                
                InboxMessages = allMessages
                    .Where(x => x.MessageType != "Sent")
                    .ToList(),

               
                OutboxMessages = allMessages
                    .Where(x => x.MessageType == "Sent")
                    .ToList()
            };

            return View(model);
        }

  
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var message = await _contactService.GetMessageByIdAsync(id);

            if (message == null)
            {
                return RedirectToAction("Index");
            }

            if (!message.IsRead)
            {
                await _contactService.MarkAsReadAsync(id);
            }

            return View(message);
        }


        public async Task<IActionResult> Delete(int id)
        {
            await _contactService.DeleteMessageAsync(id);
            return RedirectToAction("Index");
        }

    
        [HttpPost]
        public async Task<IActionResult> Reply(int id, string replyMessage)
        {
            await _contactService.ReplyToMessageAsync(id, replyMessage);

        
            TempData["Success"] = "Cevap başarıyla gönderildi.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? toMemberId)
        {
            var model = new CreateContactMessageDto();

            
            var members = await _memberService.GetAllMembersAsync();

           
            if (toMemberId.HasValue)
            {
                var targetMember = members.FirstOrDefault(m => m.Id == toMemberId.Value);
                if (targetMember != null)
                {
                    model.SelectedMemberId = targetMember.Id;
                    model.Email = targetMember.Email;
                    model.Name = targetMember.FullName;
                    model.Subject = "Kütüphane Ödünç İşlemi Hatırlatması"; 
                    model.Message = $"Sayın {targetMember.FullName},\n\n"; 
                }
            }

         
            ViewBag.Members = new SelectList(members, "Id", "FullName", toMemberId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContactMessageDto model)
        {
            if (ModelState.IsValid)
            {
                
                var contactDto = new ContactMessageDto
                {
                    Name = model.Name,
                    Email = model.Email,
                    Subject = model.Subject,
                    Message = model.Message,
                    MessageType = ContactMessageType.Sent
                };

              
                await _contactService.SendMessageAsync(contactDto, null);

                TempData["Success"] = "Mesaj başarıyla gönderildi.";
                return RedirectToAction("Index");
            }

          
            var members = await _memberService.GetAllMembersAsync();
            ViewBag.Members = new SelectList(members, "Id", "FullName", model.SelectedMemberId);

            return View(model);
        }

    }
}