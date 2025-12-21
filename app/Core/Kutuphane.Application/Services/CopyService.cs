using Kutuphane.Application.Dtos.CopyDtos;
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

public class CopyService:ICopyService
{
    private readonly ICopyRepository _copyRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ILoanRepository _loanRepository;

    public CopyService(
        ICopyRepository copyRepository,
        IBookRepository bookRepository,
        ILoanRepository loanRepository)
    {
        _copyRepository = copyRepository;
        _bookRepository = bookRepository;
        _loanRepository = loanRepository;
    }

    public async Task<IEnumerable<ResultCopyDto>> GetCopiesByBookIdAsync(int bookId)
    {
        var copies = await _copyRepository.FindAsync(c => c.BookId == bookId);
        return copies.Select(MapToDto);
    }

    public async Task<ResultCopyDto?> GetCopyByIdAsync(int id)
    {
        var copy = await _copyRepository.GetByIdAsync(id);
        return copy != null ? MapToDto(copy) : null;
    }

    public async Task<CopyWithDetailsDto?> GetCopyWithDetailsAsync(int id)
    {
        var copy = await _copyRepository.GetCopyWithDetailsAsync(id);
        if (copy == null)
            return null;

        return new CopyWithDetailsDto
        {
            Id = copy.Id,
            BookId = copy.BookId,
            CopyNumber = copy.CopyNumber,
            Status = copy.Status.ToString(),
            ShelfLocation = copy.ShelfLocation,
            AcquisitionDate = copy.AcquisitionDate,
            Price = copy.Price,
            Condition = copy.Condition,
            Book = new Dtos.BookDtos.ResultBookDto
            {
                Id = copy.Book.Id,
                ISBN = copy.Book.ISBN,
                Title = copy.Book.Title,
                Author = copy.Book.Author,
                Publisher = copy.Book.Publisher,
                PublicationYear = copy.Book.PublicationYear,
                Category = copy.Book.Category,
                PageCount = copy.Book.PageCount,
                Language = copy.Book.Language,
                Description = copy.Book.Description
            },
            ActiveLoan = copy.Loans.FirstOrDefault() != null ? new Dtos.LoanDtos.ResultLoanDto
            {
                Id = copy.Loans.First().Id,
                CopyId = copy.Loans.First().CopyId,
                MemberId = copy.Loans.First().MemberId,
                MemberName = copy.Loans.First().Member.FullName,
                BookTitle = copy.Book.Title,
                CopyNumber = copy.CopyNumber,
                LoanDate = copy.Loans.First().LoanDate,
                DueDate = copy.Loans.First().DueDate,
                ReturnDate = copy.Loans.First().ReturnDate,
                Status = copy.Loans.First().Status.ToString(),
                Notes = copy.Loans.First().Notes,
                LoanedByUserId = copy.Loans.First().LoanedByUserId,
                LoanedByUserName = copy.Loans.First().LoanedByUser.FullName,
                IsOverdue = copy.Loans.First().IsOverdue,
                OverdueDays = copy.Loans.First().OverdueDays
            } : null
        };
    }

    public async Task<IEnumerable<ResultCopyDto>> GetAvailableCopiesAsync(int bookId)
    {
        var copies = await _copyRepository.GetAvailableCopiesAsync(bookId);
        return copies.Select(MapToDto);
    }

    public async Task<ResultCopyDto> CreateCopyAsync(CreateCopyDto dto)
    {
        // Kitap var mı kontrol et
        var book = await _bookRepository.GetByIdAsync(dto.BookId);
        if (book == null)
        {
            throw new NotFoundException("Book", dto.BookId);
        }

        // Aynı kopya numarası var mı kontrol et
        var existingCopy = await _copyRepository.FirstOrDefaultAsync(
            c => c.BookId == dto.BookId && c.CopyNumber == dto.CopyNumber);

        if (existingCopy != null)
        {
            throw new DuplicateException($"Copy number '{dto.CopyNumber}' already exists for this book.");
        }

        var copy = new Copy
        {
            BookId = dto.BookId,
            CopyNumber = dto.CopyNumber,
            Status = CopyStatus.Rafta,
            ShelfLocation = dto.ShelfLocation,
            AcquisitionDate = dto.AcquisitionDate,
            Price = dto.Price,
            Condition = dto.Condition
        };

        await _copyRepository.AddAsync(copy);

        return MapToDto(copy);
    }

    public async Task<ResultCopyDto> UpdateCopyAsync(int id, UpdateCopyDto dto)
    {
      
        var copy = await _copyRepository.GetByIdAsync(id);
        if (copy == null)
        {
            throw new Exception($"ID'si {id} olan kopya bulunamadı.");
        }

      
        if (Enum.TryParse<CopyStatus>(dto.Status, true, out var newStatus))
        {
            if (copy.Status == CopyStatus.Oduncte && newStatus != CopyStatus.Oduncte)
            {
                var activeLoan = await _loanRepository.GetActiveLoanAsync(id);
                if (activeLoan != null)
                {
                    throw new Exception("Bu kopya şu an bir üyede (Ödünçte). Durumunu değiştirmek için önce kitabı iade almalısınız.");
                }
            }
            copy.Status = newStatus;
        }

        copy.ShelfLocation = dto.ShelfLocation;
        copy.Condition = dto.Condition;

        await _copyRepository.UpdateAsync(copy);


        return MapToDto(copy);
    }
    public async Task DeleteCopyAsync(int copyId)
    {
        var copy = await _copyRepository.GetByIdAsync(copyId);
        if (copy == null) throw new NotFoundException("Copy", copyId);

        
        if (copy.Status == CopyStatus.Oduncte)
        {
            throw new BusinessException("Bu kopya şu an bir üyede. Silmeden önce iade almalısınız.");
        }

        
        copy.IsDeleted = true;
        await _copyRepository.UpdateAsync(copy);
    }

    private ResultCopyDto MapToDto(Copy copy)
    {
        return new ResultCopyDto
        {
            Id = copy.Id,
            BookId = copy.BookId,
            CopyNumber = copy.CopyNumber,
            Status = copy.Status.ToString(),
            ShelfLocation = copy.ShelfLocation,
            AcquisitionDate = copy.AcquisitionDate,
            Price = copy.Price,
            Condition = copy.Condition
        };
    }

    public async Task AddCopiesAsync(int bookId, int count, string shelfLocation)
    {
     
        var lastCopy = await _copyRepository.GetLastCopyOfBookAsync(bookId);

        int startNumber = 1;
        if (lastCopy != null)
        {
            if (int.TryParse(lastCopy.CopyNumber, out int lastNumber))
            {
                startNumber = lastNumber + 1;
            }
        }

        for (int i = 0; i < count; i++)
        {
            var copy = new Copy
            {
                BookId = bookId,
          
                CopyNumber = (startNumber + i).ToString("D3"),
                Status = CopyStatus.Rafta,
                ShelfLocation = shelfLocation,
                IsDeleted = false,
                AcquisitionDate = DateTime.Now,
                Condition = "Yeni",
                Price = 0,
            };

            await _copyRepository.AddAsync(copy);
        }
    }
}
