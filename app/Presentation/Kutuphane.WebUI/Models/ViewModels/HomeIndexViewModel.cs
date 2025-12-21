using Kutuphane.Application.Dtos.BookDtos;

namespace Kutuphane.WebUI.Models.ViewModels
{
    public class HomeIndexViewModel
    {
 
        public IEnumerable<ResultBookDto> NewBooks { get; set; } = new List<ResultBookDto>();

        public IEnumerable<ResultBookDto> PopularBooks { get; set; } = new List<ResultBookDto>();

        public IEnumerable<CategoryStatViewModel> Categories { get; set; } = new List<CategoryStatViewModel>();
    }

    public class CategoryStatViewModel
    {
        public string CategoryName { get; set; }
        public int BookCount { get; set; }
        public string IconCssClass { get; set; }
    }
}
