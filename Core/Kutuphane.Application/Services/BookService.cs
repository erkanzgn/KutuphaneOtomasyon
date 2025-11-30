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

    public BookService(IBookRepository bookRepository, ICopyRepository copyRepository)
    {
        _bookRepository = bookRepository;
        _copyRepository = copyRepository;
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
        if (await _bookRepository.IsISBNExistsAsync(dto.ISBN))
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
        if (book == null)
        {
            throw new NotFoundException("Book", id);
        }

        book.Title = dto.Title;
        book.Author = dto.Author;
        book.Publisher = dto.Publisher;
        book.PublicationYear = dto.PublicationYear;
        book.Category = dto.Category;
        book.PageCount = dto.PageCount;
        book.Language = dto.Language;
        book.ImageUrl = dto.ImageUrl;
        book.Description = dto.Description;

        await _bookRepository.UpdateAsync(book);

        var availableCopies = await _copyRepository.GetAvailableCopyCountAsync(id);
        var totalCopies = await _copyRepository.CountAsync(c => c.BookId == id);

        return MapToDto(book, totalCopies, availableCopies);
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            throw new NotFoundException("Book", id);
        }

        // Kopyası var mı kontrol et
        var hasCopies = await _copyRepository.AnyAsync(c => c.BookId == id);
        if (hasCopies)
        {
            throw new BusinessException("Cannot delete book with existing copies. Delete copies first.");
        }

        await _bookRepository.DeleteAsync(id);
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
