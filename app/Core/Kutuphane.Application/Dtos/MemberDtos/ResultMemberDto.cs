using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.MemberDtos;

public class ResultMemberDto
{
    public int Id { get; set; }
    public string MemberNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public string Status { get; set; }
    public string? Notes { get; set; }
    public DateTime? BanExpirationDate { get; set; }
}
