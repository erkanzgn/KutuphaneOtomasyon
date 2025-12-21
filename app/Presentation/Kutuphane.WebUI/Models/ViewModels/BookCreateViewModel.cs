using System.ComponentModel.DataAnnotations;

namespace Kutuphane.WebUI.Models.ViewModels
{
    public class BookCreateViewModel
    {
        [Required(ErrorMessage = "Kitap başlığı zorunludur.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Yazar adı zorunludur.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "ISBN zorunludur.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "ISBN tam olarak 13 haneli olmalıdır.")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ISBN sadece rakamlardan oluşmalıdır.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Kategori seçiniz.")]
        public string Category { get; set; }

        public int? PageCount { get; set; }

        [Display(Name = "Yayın Yılı")]
        public int? PublicationYear { get; set; }

        public string? Publisher { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Kapak Resmi")]
        public IFormFile? ImageFile { get; set; }
    }
}