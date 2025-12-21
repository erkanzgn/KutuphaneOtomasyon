using Kutuphane.Application.Dtos.ReportDtos;
using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Domain.Enums;
using System.Globalization;

namespace Kutuphane.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICopyRepository _copyRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ILoanRepository _loanRepository;

        public ReportService(
            IBookRepository bookRepository,
            ICopyRepository copyRepository,
            IMemberRepository memberRepository,
            ILoanRepository loanRepository)
        {
            _bookRepository = bookRepository;
            _copyRepository = copyRepository;
            _memberRepository = memberRepository;
            _loanRepository = loanRepository;
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            var totalBooks = await _bookRepository.CountAsync();
            var totalCopies = await _copyRepository.CountAsync();
            var availableCopies = await _copyRepository.CountAsync(c => c.Status == CopyStatus.Rafta);
            var totalMembers = await _memberRepository.CountAsync();
            var activeMembers = await _memberRepository.CountAsync(m => m.Status == MemberStatus.Aktif);
            var activeLoans = await _loanRepository.CountAsync(l => l.ReturnDate == null);

            var overdueLoans = (await _loanRepository.GetOverdueLoansAsync()).Count();

            return new DashboardStatisticsDto
            {
                TotalBooks = totalBooks,
                TotalCopies = totalCopies,
                AvailableCopies = availableCopies,
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                ActiveLoans = activeLoans,
                OverdueLoans = overdueLoans
            };
        }

        public async Task<IEnumerable<OverdueLoanReportDto>> GetOverdueLoansReportAsync()
        {
            var overdueLoans = await _loanRepository.GetOverdueLoansAsync();

            return overdueLoans.Select(loan => new OverdueLoanReportDto
            {
                LoanId = loan.Id,
                MemberName = loan.Member?.FullName ?? "Bilinmiyor",
                MemberPhone = loan.Member?.Phone ?? "-",
                BookTitle = loan.Copy?.Book?.Title ?? "Bilinmiyor",
                CopyNumber = loan.Copy?.CopyNumber ?? "-",
                DueDate = loan.DueDate,
                OverdueDays = loan.OverdueDays
            });
        }

        public async Task<IEnumerable<PopularBookDto>> GetMostBorrowedBooksAsync(int count)
        {
            var books = await _bookRepository.GetMostBorrowedBooksAsync(count);

            return books.Select(book => new PopularBookDto
            {
                BookId = book.Id,
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                BorrowCount = book.Copies?.SelectMany(c => c.Loans).Count() ?? 0
            });
        }

        public async Task<IEnumerable<ActiveMemberDto>> GetActiveMembersAsync()
        {
            var members = await _memberRepository.FindAsync(m => m.Status == MemberStatus.Aktif);
            var activeMemberDtos = new List<ActiveMemberDto>();

            // Not: Bu döngü performansı düşürebilir (N+1 problemi). 
            // İleride Repository içinde tek bir SQL sorgusu ile çekmek daha iyidir.
            foreach (var member in members)
            {
                var activeLoans = await _loanRepository.CountAsync(l => l.MemberId == member.Id && l.ReturnDate == null);
                var totalLoans = await _loanRepository.CountAsync(l => l.MemberId == member.Id);

                activeMemberDtos.Add(new ActiveMemberDto
                {
                    MemberId = member.Id,
                    MemberNumber = member.MemberNumber,
                    FullName = member.FullName,
                    ActiveLoanCount = activeLoans,
                    TotalLoanCount = totalLoans
                });
            }

            return activeMemberDtos.OrderByDescending(m => m.TotalLoanCount);
        }

        public async Task<MonthlyStatisticsDto> GetMonthlyLoanStatisticsAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var statistics = await _loanRepository.GetLoanStatisticsAsync(startDate, endDate);

            var newMembers = await _memberRepository.CountAsync(m =>
                m.RegistrationDate >= startDate && m.RegistrationDate <= endDate);

            var newBooks = await _bookRepository.CountAsync(b =>
                b.CreatedDate >= startDate && b.CreatedDate <= endDate);

            return new MonthlyStatisticsDto
            {
                Year = year,
                Month = month,
                TotalLoans = statistics.GetValueOrDefault("TotalLoans", 0),
                TotalReturns = statistics.GetValueOrDefault("TotalReturns", 0),
                NewMembers = newMembers,
                NewBooks = newBooks
            };
        }

        public async Task<AdminStatsDto> GetDetailedStatsAsync()
        {
            // 1. Tüm Ödünç Verilerini Çek
            // (Not: LoanRepository.GetAllAsync içinde Member ve Book Include edildiğinden emin olmalısın)
            var allLoans = await _loanRepository.GetAllAsync();

            // --- GRAFİK 1: AYLIK ÖDÜNÇ TRENDİ ---
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            // Entity'de LoanDate zorunlu (DateTime) olduğu için direkt kullanıyoruz
            var recentLoans = allLoans
                .Where(l => l.LoanDate >= sixMonthsAgo)
                .ToList();

            var groupedLoans = recentLoans
                .GroupBy(l => new { l.LoanDate.Year, l.LoanDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    Count = g.Count()
                })
                .ToList();

            // 2. Tüm Kitapları Çek
            var allBooks = await _bookRepository.GetAllAsync();

            // --- GRAFİK 2: KATEGORİ DAĞILIMI ---
            var categoryStats = allBooks
                .GroupBy(b => b.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            // --- GRAFİK 3: EN POPÜLER YAZARLAR (YENİ) ---
            // Kütüphanede en çok kitabı bulunan yazarları buluyoruz
            var topAuthorsData = allBooks
                .GroupBy(b => b.Author)
                .Select(g => new { Author = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            // --- TABLO: EN ÇOK OKUYANLAR ---
            var topReaders = allLoans
                .Where(l => l.MemberId > 0)
                .GroupBy(l => l.MemberId)
                .Select(g => new TopReaderDto
                {
                    // Null check (??) ile veri güvenliği sağlıyoruz
                    MemberName = g.First().Member?.FullName ?? "Silinmiş Üye",
                    MemberNumber = g.First().Member?.MemberNumber ?? "-",
                    TotalReadCount = g.Count()
                })
                .OrderByDescending(x => x.TotalReadCount)
                .Take(5)
                .ToList();

            // --- KPI KARTLARI (ÖZET BİLGİLER) ---
            var totalMembers = await _memberRepository.CountAsync(); // Toplam üye sayısı
            var activeLoansCount = allLoans.Count(l => l.ReturnDate == null); // Şu an dışarıdaki kitaplar

            // --- DTO DÖNÜŞÜ ---
            return new AdminStatsDto
            {
                // Aylık Trend
                Months = groupedLoans.Select(x => x.MonthName).ToList(),
                MonthlyLoanCounts = groupedLoans.Select(x => x.Count).ToList(),

                // Kategoriler
                Categories = categoryStats.Select(x => x.Category ?? "Diğer").ToList(),
                BookCountsByCategory = categoryStats.Select(x => x.Count).ToList(),

                // Yazarlar (Yeni)
                TopAuthors = topAuthorsData.Select(x => x.Author).ToList(),
                BookCountsByAuthor = topAuthorsData.Select(x => x.Count).ToList(),

                // Tablo
                TopReaders = topReaders,

                // Özet Kartlar (Yeni)
                TotalBooks = allBooks.Count(),
                TotalMembers = totalMembers,
                ActiveLoans = activeLoansCount
            };
        }


    }
}