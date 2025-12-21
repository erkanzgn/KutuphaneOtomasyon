using Kutuphane.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Interfaces.Repositories
{
    public interface IContactMessageRepository:IGenericRepository<ContactMessage>
    {
        Task<IEnumerable<ContactMessage>> GetUnreadMessagesAsync();
        Task<IEnumerable<ContactMessage>> GetMessagesByUserIdAsync(int userId);
    }
}
