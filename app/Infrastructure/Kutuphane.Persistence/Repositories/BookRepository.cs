using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Domain.Entities;
using Kutuphane.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Repositories;

public class BookRepository:GenericRepository<Book>, IBookRepository
{
    public BookRepository(KutuphaneDbContext context) : base(context)
    {
    }

    public async Task<Book?> GetBookWithCopiesAsync(int bookId)
    {
        return await _dbSet
            .Include(b => b.Copies)
            .FirstOrDefaultAsync(b => b.Id == bookId);
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
    {
        searchTerm = searchTerm.ToLower();

        return await _dbSet
            .Where(b =>
                b.ISBN.ToLower().Contains(searchTerm) ||
                b.Title.ToLower().Contains(searchTerm) ||
                b.Author.ToLower().Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(b => b.Category == category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetMostBorrowedBooksAsync(int count)
    {
        return await _dbSet
            .Include(b => b.Copies)
                .ThenInclude(c => c.Loans)
            .OrderByDescending(b => b.Copies.SelectMany(c => c.Loans).Count())
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> IsISBNExistsAsync(string isbn, int? excludeBookId = null)
    {
        var query = _dbSet.Where(b => b.ISBN == isbn);

        if (excludeBookId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookId.Value);
        }

        return await query.AnyAsync();
    }


}
