using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.AuthDtos
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Password { get; set; }

        public UserRole Role { get; set; }
    }
}
