using Kutuphane.Domain.Entities;
using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Repositories;

public interface ICopyRepository:IGenericRepository<Copy>
{
    Task<IEnumerable<Copy>> GetAvailableCopiesAsync(int bookId);
    Task<Copy?> GetFirstAvailableCopyAsync(int bookId);
    Task<Copy?> GetCopyWithDetailsAsync(int copyId);
    Task<IEnumerable<Copy>> GetCopiesByStatusAsync(CopyStatus copyStatus);
    Task<Copy?> GetCopyWithBookAsync(int copyId);
    Task<int> GetAvailableCopyCountAsync(int bookId);
    Task<IEnumerable<Copy>> GetAllAvailableCopiesForBookAsync(int bookId);
    Task<Copy?> GetLastCopyOfBookAsync(int bookId);

    Task<IEnumerable<Copy>> GetAllCopiesOfBookAsync(int bookId);
}
