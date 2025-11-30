using Kutuphane.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kutuphane.WebUI.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, string category, string author, string pages, bool available)
        {
            // Tüm kitapları al
            var allBooks = await _bookService.GetAllBooksAsync();
            var filteredBooks = allBooks.AsEnumerable();

            // 1. Kitap adına göre ara
            if (!string.IsNullOrEmpty(search))
            {
                filteredBooks = filteredBooks.Where(b =>
                    b.Title.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // 2. Kategoriye göre filtrele
            if (!string.IsNullOrEmpty(category))
            {
                filteredBooks = filteredBooks.Where(b => b.Category == category);
            }

            // 3. Yazara göre filtrele
            if (!string.IsNullOrEmpty(author))
            {
                filteredBooks = filteredBooks.Where(b =>
                    b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
            }

            // 4. Sayfa sayısına göre filtrele
            if (!string.IsNullOrEmpty(pages))
            {
                if (pages == "0-200")
                {
                    filteredBooks = filteredBooks.Where(b => b.PageCount <= 200);
                }
                else if (pages == "200-400")
                {
                    filteredBooks = filteredBooks.Where(b => b.PageCount > 200 && b.PageCount <= 400);
                }
                else if (pages == "400-600")
                {
                    filteredBooks = filteredBooks.Where(b => b.PageCount > 400 && b.PageCount <= 600);
                }
                else if (pages == "600")
                {
                    filteredBooks = filteredBooks.Where(b => b.PageCount > 600);
                }
            }

            // 5. Sadece müsait olanlar
            if (available)
            {
                filteredBooks = filteredBooks.Where(b => b.AvailableCopies > 0);
            }

            return View(filteredBooks.ToList());
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _bookService.GetBookWithCopiesAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

    }
}

