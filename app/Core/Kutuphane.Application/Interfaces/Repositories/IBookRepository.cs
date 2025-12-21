using Kutuphane.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Repositories;

public interface IBookRepository :IGenericRepository<Book>
{
    Task<Book?> GetBookWithCopiesAsync(int bookId);
    Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
    Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category);
    Task<IEnumerable<Book>> GetMostBorrowedBooksAsync(int count);
    Task<bool> IsISBNExistsAsync(string isbn, int? excludeBookId = null);
}
