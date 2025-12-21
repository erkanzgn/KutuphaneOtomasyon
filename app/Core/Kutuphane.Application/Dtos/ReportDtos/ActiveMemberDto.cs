using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.ReportDtos;

public class ActiveMemberDto
{
    public int MemberId { get; set; }
    public string MemberNumber { get; set; }
    public string FullName { get; set; }
    public int ActiveLoanCount { get; set; }
    public int TotalLoanCount { get; set; }
}
