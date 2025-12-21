using Kutuphane.Application.Dtos.BookDtos;
using Kutuphane.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Services;

public interface IBookService
{
    Task<IEnumerable<ResultBookDto>> GetAllBooksAsync();
    Task<ResultBookDto?> GetBookByIdAsync(int id);
    Task<BookWithCopiesDto?> GetBookWithCopiesAsync(int id);
    Task<IEnumerable<ResultBookDto>> SearchBooksAsync(string searchTerm);
    Task<IEnumerable<ResultBookDto>> GetBooksByCategoryAsync(string category);
    Task<ResultBookDto> CreateBookAsync(CreateBookDto dto);
    Task<ResultBookDto> UpdateBookAsync(int id, UpdateBookDto dto);
    Task DeleteBookAsync(int id);
}
