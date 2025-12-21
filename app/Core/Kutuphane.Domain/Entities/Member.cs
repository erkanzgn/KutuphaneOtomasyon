using Kutuphane.Domain.Common;
using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Domain.Entities
{
    public class Member:BaseEntity
    {
        public string MemberNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public MemberStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? BanExpirationDate { get; set; }

        public virtual ICollection<Loan> Loans { get; set; }

        public Member()
        {
            Loans = new HashSet<Loan>();
        }
        public string FullName => $"{FirstName} {LastName}";
    }
}
