using Kutuphane.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Domain.Entities
{
    public class Book:BaseEntity
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string? Publisher { get; set; }
        public int? PublicationYear { get; set; }
        public string? Category { get; set; }
        public int? PageCount { get; set; }
        public string Language { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public virtual ICollection<Copy> Copies { get; set; }

        public Book()
        {
            //HashSet, bir değeri sadece bir kez saklar ve aynı değeri birden fazla kez eklemeye izin vermez
            Copies = new HashSet<Copy>();
        }
    }
}
