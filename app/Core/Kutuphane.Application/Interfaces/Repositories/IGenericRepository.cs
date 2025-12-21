using Kutuphane.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class 
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T,bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T,bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T,bool>> predicate);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T,bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<IEnumerable<T>> GetPageAsync(int pageNumber ,int pageSize);
}
