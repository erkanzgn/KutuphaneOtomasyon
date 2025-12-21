using Kutuphane.Application.Dtos.AuthDtos;
using Kutuphane.Application.Dtos.BookDtos;
using Kutuphane.Application.Dtos.LoanDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.MemberDtos;

public class MemberWithLoansDto:ResultMemberDto
{
    public IEnumerable<ResultLoanDto> ActiveLoans { get; set; } = new List<ResultLoanDto>();
    public IEnumerable<ResultLoanDto> LoanHistory { get; set; } = new List<ResultLoanDto>();
}
