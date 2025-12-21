using Kutuphane.Application.Dtos.AuthDtos;
using Kutuphane.Application.Dtos.LoanDtos;
using Kutuphane.Application.Exceptions;
using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Domain.Entities;
using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly ICopyRepository _copyRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUserRepository _userRepository;
    private const int LoanDurationDays = 14;

    public LoanService(
        ILoanRepository loanRepository,
        ICopyRepository copyRepository,
        IMemberRepository memberRepository,
        IUserRepository userRepository)
    {
        _loanRepository = loanRepository;
        _copyRepository = copyRepository;
        _memberRepository = memberRepository;
        _userRepository = userRepository;
    }

    public async Task<ResultLoanDto> BorrowBookAsync(CreateLoanDto dto)
    {
        var member = await _memberRepository.GetByIdAsync(dto.MemberId);

        
        if (member.Status != MemberStatus.Aktif)
        {
            throw new MemberNotEligibleException("Üyeliğiniz aktif olmadığı için kitap alamazsınız. Yönetimle iletişime geçin.");
        }

      
        if (member.BanExpirationDate.HasValue && member.BanExpirationDate > DateTime.Now)
        {
            var remainingTime = member.BanExpirationDate.Value - DateTime.Now;
            int remainingDays = (int)remainingTime.TotalDays;

            string timeText = remainingDays > 0 ? $"{remainingDays} gün" : "birkaç saat";

            throw new MemberNotEligibleException($"Cezanız bulunmaktadır. {member.BanExpirationDate.Value:dd.MM.yyyy} tarihine kadar ({timeText} daha) kitap ödünç alamazsınız.");
        }
        if (dto.CopyId == 0 && dto.BookId != 0)
        {
           
            var candidateCopies = await _copyRepository.GetAllAvailableCopiesForBookAsync(dto.BookId);

            if (!candidateCopies.Any())
            {
                throw new InvalidOperationException("Bu kitaba ait müsait kopya bulunamadı.");
            }

            bool copyFound = false;

            foreach (var kopya in candidateCopies)
            {

                if (await CanCopyBeBorrowedAsync(kopya.Id))
                {
                    dto.CopyId = kopya.Id; 
                    copyFound = true;
                    break;
                }
         
            }

            if (!copyFound)
            {
                throw new InvalidOperationException("Sistemde müsait kopyalar görünmesine rağmen teknik bir tutarsızlık nedeniyle işlem yapılamadı.");
            }
        }
        // 1. Kopya kontrolü
        var copy = await _copyRepository.GetCopyWithBookAsync(dto.CopyId);
        if (copy == null) throw new NotFoundException("Copy", dto.CopyId);

        // 2. Kopya müsait mi?
        if (!await CanCopyBeBorrowedAsync(dto.CopyId)) throw new CopyNotAvailableException(dto.CopyId);

        // 3. Üye kontrolü
        var memberr = await _memberRepository.GetByIdAsync(dto.MemberId);
        if (memberr == null) throw new NotFoundException("Member", dto.MemberId);

        // 4. Üye ödünç alabilir mi? (Aktiflik Kontrolü)
        if (member.Status != MemberStatus.Aktif)
        {
            throw new MemberNotEligibleException($"Üye durumu '{member.Status}'. Sadece aktif üyeler ödünç alabilir.");
        }
        // Üyenin elindeki tüm kitapları (aktif ödünçleri) çekiyoruz
        var activeLoans = await _loanRepository.GetMemberActiveLoansAsync(dto.MemberId);

        // KURAL A: Maksimum 2 Kitap Kontrolü 
        if (activeLoans.Count() >= 2)
        {  
            throw new MemberNotEligibleException("Ödünç alma limitine ulaştınız (Maks: 2). Yeni kitap almak için lütfen elinizdeki kitaplardan birini iade ediniz.");
        }

        // KURAL B: Gecikmiş Kitap Kontrolü
        var hasOverdueLoans = activeLoans.Any(l => l.IsOverdue);
        if (hasOverdueLoans)
        {
            throw new MemberNotEligibleException("Gecikmiş iadeniz bulunduğu için yeni kitap ödünç alamazsınız.");
        }

        // KURAL C: Aynı kitabı tekrar almama kontrolü (İsteğe bağlı ek güvenlik)
        // Eğer üye aynı kitabın farklı bir kopyasını almaya çalışıyorsa engelleyebilirsin.
        if (activeLoans.Any(l => l.Copy.BookId == copy.BookId))
        {
            throw new MemberNotEligibleException("Bu kitabı zaten ödünç almışsınız. Aynı kitabı iki kez alamazsınız.");
        }

     

        // 6. Kullanıcı (Personel/Sistemi Kullanan) kontrolü
        var user = await _userRepository.GetByIdAsync(dto.LoanedByUserId);
        if (user == null) throw new NotFoundException("User", dto.LoanedByUserId);

        // 7. Ödünç kaydı oluştur
        var loan = new Loan
        {
            CopyId = dto.CopyId,
            MemberId = dto.MemberId,
            LoanDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(LoanDurationDays),
            Status = LoanStatus.Aktif,
            Notes = dto.Notes,
            LoanedByUserId = dto.LoanedByUserId
        };

        await _loanRepository.AddAsync(loan);

        // 8. Kopya durumunu güncelle
        copy.Status = CopyStatus.Oduncte;
        await _copyRepository.UpdateAsync(copy);

        // 9. DTO döndür
        return new ResultLoanDto
        {
            Id = loan.Id,
            CopyId = loan.CopyId,
            MemberId = loan.MemberId,
            MemberName = member.FullName,
            BookTitle = copy.Book.Title,
            CopyNumber = copy.CopyNumber,
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status.ToString(),
            Notes = loan.Notes,
            BookId = copy.BookId,
            LoanedByUserId = loan.LoanedByUserId,
            LoanedByUserName = user.FullName,
            IsOverdue = loan.IsOverdue,
            OverdueDays = loan.OverdueDays
        };
    }


  
    private const int PenaltyMultiplier = 3; 
    private const int PermanentBanLimitDays = 30; 

    public async Task<ResultLoanDto> ReturnBookAsync(int loanId, ReturnLoanDto dto)
    {
        var loan = await _loanRepository.GetLoanWithDetailsAsync(loanId);
        if (loan == null) throw new NotFoundException("Loan", loanId);

        if (loan.ReturnDate != null)
            throw new BusinessException("Bu kitap zaten iade edilmiş.");

       
        var returnDate = DateTime.Now;
        loan.ReturnDate = returnDate;
        loan.Status = LoanStatus.IadeEdildi;

        if (returnDate > loan.DueDate)
        {
            var overdueSpan = returnDate - loan.DueDate; 
            int overdueDays = (int)overdueSpan.TotalDays;

            if (overdueDays > 0)
            {
               
                if (overdueDays >= PermanentBanLimitDays)
                {
                    loan.Member.Status = MemberStatus.Pasif;
                    loan.Member.Notes += $" | {DateTime.Now}: {overdueDays} gün gecikme nedeniyle SİSTEM TARAFINDAN KALICI ENGELLENDİ.";
                    loan.Member.BanExpirationDate = DateTime.Now.AddYears(100);
                }
          
                else
                {
                    int banDays = overdueDays * PenaltyMultiplier;


                    DateTime startBanDate = DateTime.Now;

                    if (loan.Member.BanExpirationDate.HasValue && loan.Member.BanExpirationDate.Value > DateTime.Now)
                    {
                        startBanDate = loan.Member.BanExpirationDate.Value;
                    }

                    loan.Member.BanExpirationDate = startBanDate.AddDays(banDays);

                    loan.Notes += $" | {overdueDays} gün gecikme. {banDays} gün kitap alma yasağı verildi.";
                }

                await _memberRepository.UpdateAsync(loan.Member);
            }
        }


        if (!string.IsNullOrWhiteSpace(dto.Notes)) loan.Notes += " | Not: " + dto.Notes;

        await _loanRepository.UpdateAsync(loan);

      
        var copy = await _copyRepository.GetByIdAsync(loan.CopyId);
        if (copy != null) { copy.Status = CopyStatus.Rafta; await _copyRepository.UpdateAsync(copy); }

        return MapToDto(loan);
    }

    public async Task<ResultLoanDto?> GetLoanByIdAsync(int id)
    {
        var loan = await _loanRepository.GetLoanWithDetailsAsync(id);
        return loan != null ? MapToDto(loan) : null;
    }

    public async Task<ResultLoanDto?> GetActiveLoanAsync(int copyId)
    {
        var loan = await _loanRepository.GetActiveLoanAsync(copyId);
        return loan != null ? MapToDto(loan) : null;
    }

    public async Task<IEnumerable<ResultLoanDto>> GetMemberActiveLoansAsync(int memberId)
    {
        var loans = await _loanRepository.GetMemberActiveLoansAsync(memberId);
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<ResultLoanDto>> GetOverdueLoansAsync()
    {
        var loans = await _loanRepository.GetOverdueLoansAsync();
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<ResultLoanDto>> GetLoanHistoryAsync(int memberId, int pageNumber, int pageSize)
    {
        var loans = await _loanRepository.GetLoanHistoryAsync(memberId, pageNumber, pageSize);
        return loans.Select(MapToDto);
    }

    public async Task<bool> CanCopyBeBorrowedAsync(int copyId)
    {
        var copy = await _copyRepository.GetByIdAsync(copyId);
        if (copy == null)
            return false;

        // Kopya rafta mı?
        if (copy.Status != CopyStatus.Rafta)
            return false;

        // Aktif ödünç var mı?
        var activeLoan = await _loanRepository.GetActiveLoanAsync(copyId);
        return activeLoan == null;
    }

    public async Task<int> CalculateOverdueDaysAsync(int loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan == null)
        {
            throw new NotFoundException("Loan", loanId);
        }

        return loan.OverdueDays;
    }

    private ResultLoanDto MapToDto(Loan loan)
    {
        return new ResultLoanDto
        {
            Id = loan.Id,
            CopyId = loan.CopyId,
            MemberId = loan.MemberId,
            MemberName = loan.Member?.FullName ?? "Silinmiş Üye",
            BookTitle = loan.Copy?.Book?.Title ?? "Bilinmeyen Kitap",
            CopyNumber = loan.Copy?.CopyNumber ?? "---",
            Author = loan.Copy?.Book?.Author ?? "",

            BookId = loan.Copy?.BookId ?? 0,
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status.ToString(),
            Notes = loan.Notes,
            LoanedByUserId = loan.LoanedByUserId,
            LoanedByUserName = loan.LoanedByUser?.FullName ?? "Sistem/Bilinmiyor",
            IsOverdue = loan.IsOverdue,
            OverdueDays = loan.OverdueDays
        };
    }

    public async Task<IEnumerable<ResultLoanDto>> GetActiveLoansAsync()
    {
        var activeLoans = await _loanRepository.GetActiveLoansAsync();
        return activeLoans.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ResultLoanDto>> GetAllLoansAsync()
    {
    
        var loans = await _loanRepository.GetAllAsync();

     
        return loans.Select(MapToDto).ToList();
    }
}
