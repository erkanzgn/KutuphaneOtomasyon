using Kutuphane.Application.Dtos.ReportDtos;
using Kutuphane.Domain.Entities;

namespace Kutuphane.WebUI.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public DashboardStatisticsDto Stats { get; set; }
        public List<OverdueLoanReportDto> OverdueLoans { get; set; }
        public List<PopularBookDto> PopularBooks { get; set; }

        public List<ContactMessage> RecentMessages { get; set; } 
        public int UnreadMessageCount { get; set; }
    }
}
