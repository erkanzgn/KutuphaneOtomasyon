using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.ContactDtos
{
    public class CreateContactMessageDto
    {
        
        public int? SelectedMemberId { get; set; }

        [Required(ErrorMessage = "E-Posta adresi gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "İsim alanı gereklidir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Konu başlığı gereklidir.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Mesaj içeriği gereklidir.")]
        public string Message { get; set; }
    }
}

