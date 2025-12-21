using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.CopyDtos;

public class UpdateCopyDto
{
    public string Status { get; set; }
    public string? ShelfLocation { get; set; }
    public string? Condition { get; set; }
}
