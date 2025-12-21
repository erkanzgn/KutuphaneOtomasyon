using Kutuphane.Application.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.MemberDtos;

public class CreateMemberDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [Required(ErrorMessage = "E-Posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta formatı giriniz.")]
    [ValidEmailDomain(ErrorMessage = "Girdiğiniz e-posta adresinin alan adı (domain) geçersiz.")]
    public string? Email { get; set; }
    public string Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Notes { get; set; }
}
