using Kutuphane.Domain.Common;
using Kutuphane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Domain.Entities
{
    public class Loan : BaseEntity
    {
        public int CopyId { get; set; }
        public int MemberId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public LoanStatus Status { get; set; }
        public string? Notes { get; set; }
        public int LoanedByUserId { get; set; }

        public virtual Copy Copy { get; set; }
        public virtual Member Member { get; set; }
        public virtual User LoanedByUser { get; set; }

        public bool IsOverdue => ReturnDate == null && DateTime.Now.Date > DueDate.Date;

        public int OverdueDays
        {
            get
            {

                if (ReturnDate != null) return 0;

                if (!IsOverdue) return 0;

                var days = (DateTime.Now.Date - DueDate.Date).Days;

                return days > 0 ? days : 0;
            }
        }
    }
}
