using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.AuthDtos;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Mevcut şifre zorunludur.")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
    public string ConfirmPassword { get; set; }
}