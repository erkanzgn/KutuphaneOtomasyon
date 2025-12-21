using Kutuphane.Application.Dtos.ReportDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Services;

public interface IReportService
{
    Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
    Task<IEnumerable<OverdueLoanReportDto>> GetOverdueLoansReportAsync();
    Task<IEnumerable<PopularBookDto>> GetMostBorrowedBooksAsync(int count);
    Task<IEnumerable<ActiveMemberDto>> GetActiveMembersAsync();
    Task<MonthlyStatisticsDto> GetMonthlyLoanStatisticsAsync(int year, int month);
    Task<AdminStatsDto> GetDetailedStatsAsync();
}
