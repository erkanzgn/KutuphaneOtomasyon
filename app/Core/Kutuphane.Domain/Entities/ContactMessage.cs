using Kutuphane.Domain.Common;
using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Domain.Entities
{
    public class ContactMessage:BaseEntity
    {
        public string  Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public ContactMessageType  MessageType  { get; set; }
        public string  Message { get; set; }
        public bool IsRead { get; set; } = false;
        public string? AdminReplay { get; set; }
        public DateTime? ReplayDate { get; set; }
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
