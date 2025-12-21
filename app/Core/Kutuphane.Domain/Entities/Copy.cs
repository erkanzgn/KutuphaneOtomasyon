using Kutuphane.Domain.Common;
using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Domain.Entities
{
    public class Copy:BaseEntity
    {
        public int BookId { get; set; }
        public string CopyNumber { get; set; }
        public CopyStatus Status { get; set; }
        public string? ShelfLocation { get; set; }
        public DateTime? AcquisitionDate { get; set; }
        public decimal? Price { get; set; }
        public string? Condition { get; set; }

        public virtual Book Book { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }

        public Copy()
        {
            Loans = new HashSet<Loan>();
        }
    }
}
