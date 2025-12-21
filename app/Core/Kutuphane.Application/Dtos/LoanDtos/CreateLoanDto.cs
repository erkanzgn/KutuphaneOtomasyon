using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.LoanDtos;

public class CreateLoanDto
{
    public int CopyId { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public int LoanedByUserId { get; set; }
    public string? Notes { get; set; }
}
