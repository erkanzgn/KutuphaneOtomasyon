using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.ReportDtos;

public class MonthlyStatisticsDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalLoans { get; set; }
    public int TotalReturns { get; set; }
    public int NewMembers { get; set; }
    public int NewBooks { get; set; }
}
