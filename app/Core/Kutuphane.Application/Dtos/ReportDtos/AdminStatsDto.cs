using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.ReportDtos
{
    public class AdminStatsDto
    {
        public List<string> Months { get; set; }
        public List<int> MonthlyLoanCounts { get; set; }
        public List<string> Categories { get; set; }
        public List<int> BookCountsByCategory { get; set; }

        public List<TopReaderDto> TopReaders { get; set; }
        public List<string> TopAuthors { get; set; }        // Yazar İsimleri
        public List<int> BookCountsByAuthor { get; set; }
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveLoans { get; set; }
    }

    public class TopReaderDto
    {
        public string MemberName { get; set; }
        public string MemberNumber { get; set; }
        public int TotalReadCount { get; set; }
   
    }
}
