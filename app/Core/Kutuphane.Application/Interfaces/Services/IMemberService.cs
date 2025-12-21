using Kutuphane.Application.Dtos.MemberDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kutuphane.Application.Dtos.MemberDtos.UpdateMemberDto;

namespace Kutuphane.Application.Interfaces.Services;

public interface IMemberService
{
    Task<IEnumerable<ResultMemberDto>> GetAllMembersAsync();
    Task<ResultMemberDto?> GetMemberByIdAsync(int id);
    Task<MemberWithLoansDto?> GetMemberWithLoansAsync(int id);
    Task<IEnumerable<ResultMemberDto>> SearchMembersAsync(string searchTerm);
    Task<bool> CanMemberBorrowAsync(int memberId);
    Task<MemberStatisticsDto> GetMemberStatisticsAsync(int memberId);
    Task<ResultMemberDto> CreateMemberAsync(CreateMemberDto dto);
    Task<ResultMemberDto> UpdateMemberAsync(int id, UpdateMemberDto dto);
    Task DeleteMemberAsync(int id);
    Task UpdateMemberProfileAsync(MemberProfileDto dto);
    Task RemoveBanAsync(int memberId);
}
