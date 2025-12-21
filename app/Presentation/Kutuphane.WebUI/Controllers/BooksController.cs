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

        public async Task<IActionResult> Index(string searchTerm, string category, string author, string pages, bool available)
        {
    
            var allBooks = await _bookService.GetAllBooksAsync();
            var filteredBooks = allBooks.AsEnumerable();

    
            if (!string.IsNullOrEmpty(searchTerm))
            {
        
                filteredBooks = filteredBooks.Where(b =>
                    b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.ISBN.Contains(searchTerm) ||
             
                    b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (!string.IsNullOrEmpty(category))
                filteredBooks = filteredBooks.Where(b => b.Category == category);

            if (!string.IsNullOrEmpty(author))
                filteredBooks = filteredBooks.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(pages))
            {
                if (pages == "0-200") filteredBooks = filteredBooks.Where(b => b.PageCount <= 200);
                else if (pages == "200-400") filteredBooks = filteredBooks.Where(b => b.PageCount > 200 && b.PageCount <= 400);
                else if (pages == "400-600") filteredBooks = filteredBooks.Where(b => b.PageCount > 400 && b.PageCount <= 600);
                else if (pages == "600") filteredBooks = filteredBooks.Where(b => b.PageCount > 600);
            }

            if (available)
                filteredBooks = filteredBooks.Where(b => b.AvailableCopies > 0);

           
            ViewBag.Categories = allBooks.Select(b => b.Category).Distinct().OrderBy(x => x).ToList();

            
            ViewBag.CurrentSearch = searchTerm;
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentAuthor = author;
            ViewBag.CurrentPages = pages;
            ViewBag.CurrentAvailable = available;

            return View(filteredBooks.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _bookService.GetBookWithCopiesAsync(id);
            if (result == null) return NotFound();

            return View(result);
        }
    }
}