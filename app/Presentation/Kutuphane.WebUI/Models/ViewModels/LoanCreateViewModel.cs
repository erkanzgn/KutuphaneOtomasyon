
using System.ComponentModel.DataAnnotations;

namespace Kutuphane.WebUI.Models.ViewModels
{
    public class LoanCreateViewModel
    {
        [Required(ErrorMessage = "Lütfen bir üye seçiniz.")]
        public int MemberId { get; set; }

        public bool IsAutoSelection { get; set; } = true; 

        public int? BookId { get; set; }


        public int? CopyId { get; set; }

        public string? Notes { get; set; }
    }
}
