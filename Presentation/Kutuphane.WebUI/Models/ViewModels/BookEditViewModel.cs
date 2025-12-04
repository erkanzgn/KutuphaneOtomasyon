using System.ComponentModel.DataAnnotations;

namespace Kutuphane.WebUI.Models.ViewModels
{
    public class BookEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kitap başlığı zorunludur.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Yazar adı zorunludur.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "ISBN zorunludur.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "ISBN 13 haneli olmalıdır.")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "Sadece rakam giriniz.")]
        public string ISBN { get; set; }

        [Required]
        public string Category { get; set; }

        public int PageCount { get; set; }
        public int PublicationYear { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Yeni Kapak Resmi (Değiştirmek isterseniz seçin)")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }
    }
}