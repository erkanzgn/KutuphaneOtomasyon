using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Domain.Entities;
using Kutuphane.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Repositories;

public class UserRepository:GenericRepository<User>,IUserRepository
{
    public UserRepository(KutuphaneDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> IsUsernameExistsAsync(string username, int? excludeUserId = null)
    {
        var query = _dbSet.Where(u => u.Username == username);

        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
    {
        var query = _dbSet.Where(u => u.Email == email);

        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync();
    }
}
