using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.MemberDtos;

public class MemberStatisticsDto
{
    public int MemberId { get; set; }
    public string MemberName { get; set; }
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
    public int CompletedLoans { get; set; }
}
