using Kutuphane.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Repositories;

public interface IMemberRepository : IGenericRepository<Member>
{
    Task<Member?> GetMemberWithLoansAsync(int memberId);
    Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm);
    Task<IEnumerable<Member>> GetMembersWithOverdueLoansAsync();
    Task<Member?> GetMemberByMemberNumberAsync(string memberNumber);
    Task<bool> IsMemberNumberExistsAsync(string memberNumber, int? excludeMemberId = null);
    Task<bool> HasActiveLoansAsync(int memberId);
}
