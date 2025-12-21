using Kutuphane.Domain.Enums; // <--- Bu satırın en üstte olduğundan emin ol
using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Application.Dtos.ContactDtos
{
    public class ContactMessageDto
    {
        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        public string Name { get; set; }

        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz.")]
        public string Email { get; set; }

        [Display(Name = "Konu")]
        [Required(ErrorMessage = "Konu başlığı zorunludur.")]
        public string Subject { get; set; }

        [Display(Name = "Mesaj Türü")]
        [Required(ErrorMessage = "Lütfen bir mesaj türü seçiniz.")]
        public ContactMessageType MessageType { get; set; }

        [Display(Name = "Mesajınız")]
        [Required(ErrorMessage = "Mesaj içeriği zorunludur.")]
        [MinLength(10, ErrorMessage = "Mesajınız en az 10 karakter olmalıdır.")]
        public string Message { get; set; }
    }
}