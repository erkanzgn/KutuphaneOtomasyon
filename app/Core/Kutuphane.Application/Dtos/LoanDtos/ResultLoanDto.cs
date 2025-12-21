using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.LoanDtos;

public class ResultLoanDto
{
    public int Id { get; set; }
    public int CopyId { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; }
    public string BookTitle { get; set; }
    public string CopyNumber { get; set; }
    public string?  Author { get; set; }
    public int BookId { get; set; }
    public DateTime? LoanDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; }
    public string? Notes { get; set; }
    public int LoanedByUserId { get; set; }
    public string LoanedByUserName { get; set; }
    public bool IsOverdue { get; set; }
    public int OverdueDays { get; set; }
}
