using Kutuphane.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Repositories;

public interface ILoanRepository:IGenericRepository<Loan>
{
    Task<Loan?> GetActiveLoanAsync(int copyId);
    Task<IEnumerable<Loan?>> GetActiveLoansAsync();
    Task<IEnumerable<Loan>> GetMemberActiveLoansAsync(int memberId);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<IEnumerable<Loan>> GetLoanHistoryAsync(int memberId , int pageNumber ,int pageSize);
    Task<Loan?> GetLoanWithDetailsAsync(int loanId);
    Task<Dictionary<string,int>> GetLoanStatisticsAsync(DateTime startDate , DateTime endDate);
}
