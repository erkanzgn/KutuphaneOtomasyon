using System.ComponentModel.DataAnnotations;

namespace Kutuphane.WebUI.Models.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [Display(Name = "Ad Soyad")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Konu zorunludur")]
        [Display(Name = "Konu")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Mesaj zorunludur")]
        [Display(Name = "Mesajınız")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
    }
}
