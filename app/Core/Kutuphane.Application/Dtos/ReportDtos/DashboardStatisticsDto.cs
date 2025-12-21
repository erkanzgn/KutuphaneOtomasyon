using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.ReportDtos;

public class DashboardStatisticsDto
{
    public int TotalBooks { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveMembers { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
}
