using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.ReportDtos;

public class OverdueLoanReportDto
{
    public int LoanId { get; set; }
    public string MemberName { get; set; }
    public string MemberPhone { get; set; }
    public string BookTitle { get; set; }
    public string CopyNumber { get; set; }
    public DateTime DueDate { get; set; }
    public int OverdueDays { get; set; }
}
