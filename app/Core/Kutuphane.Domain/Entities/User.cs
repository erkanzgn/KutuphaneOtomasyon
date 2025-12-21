using Kutuphane.Domain.Common;
using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Domain.Entities
{
    public class User:BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginDate { get; set; }

        // YENİ: User aynı zamanda Member olabilir
        public int? MemberId { get; set; }
        public virtual Member? Member { get; set; }

        // Navigation Properties
        public virtual ICollection<Loan> ProcessedLoans { get; set; }

        public User()
        {
            ProcessedLoans = new HashSet<Loan>();
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}
