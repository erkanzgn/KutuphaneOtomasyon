using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Domain.Entities;
using Kutuphane.Domain.Enums;
using Kutuphane.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Repositories
{
    public class CopyRepository:GenericRepository<Copy>,ICopyRepository
    {
        public CopyRepository(KutuphaneDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Copy>> GetAvailableCopiesAsync(int bookId)
        {
            return await _dbSet
                .Where(c => c.BookId == bookId && c.Status == CopyStatus.Rafta)
                .ToListAsync();
        }

        public async Task<Copy?> GetCopyWithDetailsAsync(int copyId)
        {
            return await _dbSet
                .Include(c => c.Book)
                .Include(c => c.Loans.Where(l => l.ReturnDate == null))
                    .ThenInclude(l => l.Member)
                .FirstOrDefaultAsync(c => c.Id == copyId);
        }

        public async Task<IEnumerable<Copy>> GetCopiesByStatusAsync(CopyStatus status)
        {
            return await _dbSet
                .Where(c => c.Status == status)
                .Include(c => c.Book)
                .ToListAsync();
        }

        public async Task<Copy?> GetCopyWithBookAsync(int copyId)
        {
            return await _dbSet
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.Id == copyId);
        }

        public async Task<int> GetAvailableCopyCountAsync(int bookId)
        {
            return await _dbSet
                .CountAsync(c => c.BookId == bookId && c.Status == CopyStatus.Rafta);
        }

        public async Task<Copy?> GetFirstAvailableCopyAsync(int bookId)
        {
            return await _dbSet
            .FirstOrDefaultAsync(x =>
                x.BookId == bookId &&
                x.Status == CopyStatus.Rafta && 
                !x.IsDeleted 
            );
        }

        public async Task<IEnumerable<Copy>> GetAllAvailableCopiesForBookAsync(int bookId)
        {
            return await _dbSet.
                Where(x =>
                x.BookId==bookId&&
                x.Status == CopyStatus.Rafta
                && !x.IsDeleted).
                ToListAsync();
        }

        public async Task<Copy?> GetLastCopyOfBookAsync(int bookId)
        {
            return await _dbSet
            .Where(x => x.BookId == bookId && !x.IsDeleted) 
            .OrderByDescending(x => x.CopyNumber)           
            .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Copy>> GetAllCopiesOfBookAsync(int bookId)
        {
            return await _dbSet.Where(x=>x.BookId==bookId&&!x.IsDeleted).ToListAsync();
        }
    }
}
