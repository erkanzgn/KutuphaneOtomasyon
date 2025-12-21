using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.CopyDtos;

public class CreateCopyDto
{
    public int BookId { get; set; }
    public string CopyNumber { get; set; }
    public string? ShelfLocation { get; set; }
    public DateTime AcquisitionDate { get; set; }
    public decimal? Price { get; set; }
    public string? Condition { get; set; }
}
