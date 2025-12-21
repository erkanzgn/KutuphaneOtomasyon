using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Kutuphane.WebUI.Models.ViewModels
{
    public class SendMessageViewModel
    {
        [Display(Name = "Alıcı Üye")]
        [Required(ErrorMessage = "Lütfen bir üye seçiniz.")]
        public string ReceiverId { get; set; }

        [Display(Name = "Konu")]
        [Required(ErrorMessage = "Konu başlığı zorunludur.")]
        public string Subject { get; set; }

        [Display(Name = "Mesaj İçeriği")]
        [Required(ErrorMessage = "Mesaj içeriği boş olamaz.")]
        public string Content { get; set; }

       
        public IEnumerable<SelectListItem>? MemberList { get; set; }
    }
}
