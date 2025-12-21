using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Domain.Entities;
using Kutuphane.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Repositories
{
    public class ContactMessageRepository : GenericRepository<ContactMessage>, IContactMessageRepository
    {
        public ContactMessageRepository(KutuphaneDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ContactMessage>> GetUnreadMessagesAsync()
        {
            return await _dbSet.Where(x => !x.IsRead).OrderByDescending(x => x.CreatedDate).ToListAsync();
        }
        public async Task<IEnumerable<ContactMessage>> GetMessagesByUserIdAsync(int userId)
        {
          
            return await _dbSet.Where(x => x.UserId == userId)
                               .OrderByDescending(x => x.CreatedDate)
                               .ToListAsync();
        }
    }
}
