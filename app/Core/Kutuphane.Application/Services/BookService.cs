using Kutuphane.Application.Dtos.BookDtos;
using Kutuphane.Application.Exceptions;
using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Services;

public class BookService:IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ICopyRepository _copyRepository;
    private readonly ILoanRepository _loanRepository;

    public BookService(IBookRepository bookRepository, ICopyRepository copyRepository, ILoanRepository loanRepository)
    {
        _bookRepository = bookRepository;
        _copyRepository = copyRepository;
        _loanRepository = loanRepository;
    }

    public async Task<IEnumerable<ResultBookDto>> GetAllBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();

        var bookDtos = new List<ResultBookDto>();
        foreach (var book in books)
        {
            var availableCopies = await _copyRepository.GetAvailableCopyCountAsync(book.Id);
            var totalCopies = await _copyRepository.CountAsync(c => c.BookId == book.Id);

            bookDtos.Add(MapToDto(book, totalCopies, availableCopies));
        }

        return bookDtos;
    }

    public async Task<ResultBookDto?> GetBookByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            return null;

        var availableCopies = await _copyRepository.GetAvailableCopyCountAsync(id);
        var totalCopies = await _copyRepository.CountAsync(c => c.BookId == id);

        return MapToDto(book, totalCopies, availableCopies);
    }

    public async Task<BookWithCopiesDto?> GetBookWithCopiesAsync(int id)
    {
        var book = await _bookRepository.GetBookWithCopiesAsync(id);
        if (book == null)
            return null;

        var availableCopies = await _copyRepository.GetAvailableCopyCountAsync(id);

        return new BookWithCopiesDto
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Author = book.Author,
            Publisher = book.Publisher,
            PublicationYear = book.PublicationYear,
            Category = book.Category,
            PageCount = book.PageCount,
            Language = book.Language,
            Description = book.Description,
            TotalCopies = book.Copies.Count,
            ImageUrl = book.ImageUrl,
            AvailableCopies = availableCopies,
            Copies = book.Copies.Select(c => new Dtos.CopyDtos.ResultCopyDto
            {
                Id = c.Id,
                BookId = c.BookId,
                CopyNumber = c.CopyNumber,
                Status = c.Status.ToString(),
                ShelfLocation = c.ShelfLocation,
                AcquisitionDate = c.AcquisitionDate,
                Price = c.Price,
                Condition = c.Condition
            })
        };
    }

    public async Task<IEnumerable<ResultBookDto>> SearchBooksAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllBooksAsync();

        var books = await _bookRepository.SearchBooksAsync(searchTerm);

        var bookDtos = new List<ResultBookDto>();
        foreach (var book in books)
        {
            var availableCopies = await _copyRepository.GetAvailableCopyCountAsync(book.Id);
            var totalCopies = await _copyRepository.CountAsync(c => c.BookId == book.Id);

            bookDtos.Add(MapToDto(book, totalCopies, availableCopies));
        }

        return bookDtos;
    }

    public async Task<IEnumerable<ResultBookDto>> GetBooksByCategoryAsync(string category)
    {
        var books = await _bookRepository.GetBooksByCategoryAsync(category);

        var bookDtos = new List<ResultBookDto>();
        foreach (var book in books)
        {
            var availableCopies = await _copyRepository.GetAvailableCopyCountAsync(book.Id);
            var totalCopies = await _copyRepository.CountAsync(c => c.BookId == book.Id);

            bookDtos.Add(MapToDto(book, totalCopies, availableCopies));
        }

        return bookDtos;
    }

    public async Task<ResultBookDto> CreateBookAsync(CreateBookDto dto)
    {
        // ISBN kontrolü
        var existingBook = await _bookRepository.FirstOrDefaultAsync(b => b.ISBN == dto.ISBN);
        if (existingBook != null)
        {
            throw new DuplicateException("Book", "ISBN", dto.ISBN);
        }

        var book = new Book
        {
            ISBN = dto.ISBN,
            Title = dto.Title,
            Author = dto.Author,
            Publisher = dto.Publisher,
            PublicationYear = dto.PublicationYear,
            Category = dto.Category,
            PageCount = dto.PageCount,
            Language = dto.Language,
            ImageUrl = dto.ImageUrl,
            Description = dto.Description
        };

        await _bookRepository.AddAsync(book);
        return MapToDto(book, 0, 0);
    }

    
    public async Task<ResultBookDto> UpdateBookAsync(int id, UpdateBookDto dto)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null) throw new NotFoundException("Book", id);

        if (book.ISBN != dto.ISBN)
        {
            var existingBook = await _bookRepository.FirstOrDefaultAsync(b => b.ISBN == dto.ISBN && b.Id != id);
            if (existingBook != null)
            {

                throw new DuplicateException("Book", "ISBN", dto.ISBN);
            }
        }


        book.Title = dto.Title;
        book.Author = dto.Author;
        book.ISBN = dto.ISBN; 
        book.Publisher = dto.Publisher;
        book.PublicationYear = dto.PublicationYear;
        book.Category = dto.Category;
        book.PageCount = dto.PageCount;
        book.Language = dto.Language;
        book.ImageUrl = dto.ImageUrl;
        book.Description = dto.Description;

        await _bookRepository.UpdateAsync(book);

      
        var totalCopies = await _copyRepository.CountAsync(c => c.BookId == id);
        var availableCopies = await _copyRepository.GetAvailableCopyCountAsync(id);

        return MapToDto(book, totalCopies, availableCopies);
    }

    public async Task DeleteBookAsync(int id)
    {
  
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null) throw new NotFoundException("Book", id);

     

        var hasActiveLoans = await _loanRepository.AnyAsync(l => l.Copy.BookId == id && l.ReturnDate == null);
        if (hasActiveLoans)
        {
            throw new BusinessException("Bu kitaba ait ödünçte olan kopyalar var. Önce iade almalısınız.");
        }

        
        book.IsDeleted = true;
        await _bookRepository.UpdateAsync(book);

     
        var copies = await _copyRepository.GetAllCopiesOfBookAsync(id); 
        foreach (var copy in copies)
        {
            copy.IsDeleted = true;
            await _copyRepository.UpdateAsync(copy); // Veya toplu update
        }
    }

    private ResultBookDto MapToDto(Book book, int totalCopies, int availableCopies)
    {
        return new ResultBookDto
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Author = book.Author,
            Publisher = book.Publisher,
            PublicationYear = book.PublicationYear,
            Category = book.Category,
            PageCount = book.PageCount,
            Language = book.Language,
            Description = book.Description,
            TotalCopies = totalCopies,
            ImageUrl = book.ImageUrl,
            AvailableCopies = availableCopies
        };
    }
}
