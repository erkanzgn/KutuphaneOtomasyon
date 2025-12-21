using Kutuphane.Application.Dtos.CopyDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.BookDtos;

public class BookWithCopiesDto:ResultBookDto
{
    public IEnumerable<ResultCopyDto> Copies { get; set; } = new List<ResultCopyDto>();
}
