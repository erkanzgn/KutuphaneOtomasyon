using Kutuphane.Application.Dtos.BookDtos;
using Kutuphane.Application.Dtos.LoanDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.CopyDtos;

public class CopyWithDetailsDto:ResultCopyDto
{
    public ResultBookDto Book { get; set; }
    public ResultLoanDto? ActiveLoan { get; set; }
}
