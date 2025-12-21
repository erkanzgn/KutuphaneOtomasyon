using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Domain.Entities;
using Kutuphane.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Repositories
{
    public class LoanRepository:GenericRepository<Loan>,ILoanRepository
    {
        public LoanRepository(KutuphaneDbContext context) : base(context)
        {
        }

        public async Task<Loan?> GetActiveLoanAsync(int copyId)
        {
            return await _dbSet
                .Include(l => l.Member)
                .Include(l => l.Copy)
                    .ThenInclude(c => c.Book)
                .FirstOrDefaultAsync(l => l.CopyId == copyId && l.ReturnDate == null);
        }

        public async Task<IEnumerable<Loan>> GetMemberActiveLoansAsync(int memberId)
        {
            return await _dbSet
                .Include(l => l.Member)       
                .Include(l => l.LoanedByUser) 
                .Include(l => l.Copy)
                    .ThenInclude(c => c.Book)
                .Where(l => l.MemberId == memberId && l.ReturnDate == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            var today = DateTime.Now;

            return await _dbSet
                .Include(l => l.Member)
                .Include(l => l.Copy)
                    .ThenInclude(c => c.Book)
                .Where(l => l.ReturnDate == null && l.DueDate < today)
                .OrderBy(l => l.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetLoanHistoryAsync(int memberId, int pageNumber, int pageSize)
        {
            return await _dbSet
                            .Include(l => l.Member)       
                            .Include(l => l.LoanedByUser)
                            .Include(l => l.Copy)
                                .ThenInclude(c => c.Book)
                            .Where(l => l.MemberId == memberId)
                            .OrderByDescending(l => l.LoanDate)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
        }

        public async Task<Loan?> GetLoanWithDetailsAsync(int loanId)
        {
            return await _dbSet
                .Include(l => l.Member)
                .Include(l => l.Copy)
                    .ThenInclude(c => c.Book)
                .Include(l => l.LoanedByUser)
                .FirstOrDefaultAsync(l => l.Id == loanId);
        }

        public async Task<Dictionary<string, int>> GetLoanStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var totalLoans = await _dbSet
                .CountAsync(l => l.LoanDate >= startDate && l.LoanDate <= endDate);

            var totalReturns = await _dbSet
                .CountAsync(l => l.ReturnDate >= startDate && l.ReturnDate <= endDate);

            var overdueLoans = await _dbSet
                .CountAsync(l => l.ReturnDate == null && l.DueDate < DateTime.Now);

            return new Dictionary<string, int>
            {
                { "TotalLoans", totalLoans },
                { "TotalReturns", totalReturns },
                { "OverdueLoans", overdueLoans }
            };
        }

        public async Task<IEnumerable<Loan?>> GetActiveLoansAsync()
        {
            return await _context.Loans
                    .Include(l => l.Member)
                    .Include(l => l.Copy)
                        .ThenInclude(c => c.Book)
                    .Include(l => l.LoanedByUser) 
                    .Where(l => l.ReturnDate == null)
                    .OrderByDescending(l => l.LoanDate) 
                    .ToListAsync();
        }
        public override async Task<IEnumerable<Loan>> GetAllAsync()
        {
         
            return await _dbSet
                .Include(l => l.Member) 
                .Include(l => l.Copy).ThenInclude(c => c.Book) 
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
