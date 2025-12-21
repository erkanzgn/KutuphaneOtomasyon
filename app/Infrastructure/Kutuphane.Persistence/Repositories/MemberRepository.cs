using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Domain.Entities;
using Kutuphane.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Repositories;

public class MemberRepository:GenericRepository<Member>,IMemberRepository
{
    public MemberRepository(KutuphaneDbContext context) : base(context)
    {
    }

    public async Task<Member?> GetMemberWithLoansAsync(int memberId)
    {
        return await _context.Members
            .Include(m => m.Loans)                
                .ThenInclude(l => l.Copy)       
                    .ThenInclude(c => c.Book)   
            .Include(m => m.Loans)
                .ThenInclude(l => l.LoanedByUser) 
            .FirstOrDefaultAsync(m => m.Id == memberId);
    }

    public async Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm)
    {
        searchTerm = searchTerm.ToLower();

        return await _dbSet
            .Where(m =>
                m.FirstName.ToLower().Contains(searchTerm) ||
                m.LastName.ToLower().Contains(searchTerm) ||
                m.Phone.Contains(searchTerm) ||
                m.MemberNumber.ToLower().Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersWithOverdueLoansAsync()
    {
        var today = DateTime.Now;

        return await _dbSet
            .Include(m => m.Loans)
            .Where(m => m.Loans.Any(l =>
                l.ReturnDate == null &&
                l.DueDate < today))
            .ToListAsync();
    }

    public async Task<Member?> GetMemberByMemberNumberAsync(string memberNumber)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.MemberNumber == memberNumber);
    }

    public async Task<bool> IsMemberNumberExistsAsync(string memberNumber, int? excludeMemberId = null)
    {
        var query = _dbSet.Where(m => m.MemberNumber == memberNumber);

        if (excludeMemberId.HasValue)
        {
            query = query.Where(m => m.Id != excludeMemberId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> HasActiveLoansAsync(int memberId)
    {
        return await _context.Loans
            .AnyAsync(l => l.MemberId == memberId && l.ReturnDate == null);
    }
}
