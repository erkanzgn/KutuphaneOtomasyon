using Kutuphane.Application.Dtos.BookDtos;
using Kutuphane.Application.Dtos.CopyDtos;

namespace Kutuphane.WebUI.Models.ViewModels
{
    public class BookAdminDetailsViewModel
    {
        public ResultBookDto Book { get; set; }
        public IEnumerable<ResultCopyDto> Copies { get; set; }
    }
}
