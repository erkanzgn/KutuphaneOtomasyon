using Kutuphane.Application.Dtos.BookDtos;
using Kutuphane.Application.Dtos.CopyDtos;
using Kutuphane.Application.Exceptions;
using Kutuphane.Application.Interfaces.Services;
using Kutuphane.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kutuphane.WebUI.Controllers.Admin
{
    [Authorize(Roles = "Admin,Librarian")]
    public class AdminBooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICopyService _copyService;

        public AdminBooksController(IBookService bookService, ICopyService copyService)
        {
            _bookService = bookService;
            _copyService = copyService;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            // Servis katmanında SearchBooksAsync yoksa GetAllBooksAsync kullanıp LINQ ile filtreleyebilirsiniz.
            var books = await _bookService.SearchBooksAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;
            return View(books);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                string? imageUrl = null;
                if (model.ImageFile != null)
                {
                    var extension = Path.GetExtension(model.ImageFile.FileName);
                    var newImageName = Guid.NewGuid() + extension;
                    var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/books");

                    if (!Directory.Exists(location)) Directory.CreateDirectory(location);

                    var fullPath = Path.Combine(location, newImageName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }
                    imageUrl = "/img/books/" + newImageName;
                }

                var createDto = new CreateBookDto
                {
                    Title = model.Title,
                    Author = model.Author,
                    ISBN = model.ISBN,
                    Category = model.Category,
                    PageCount = model.PageCount ?? 0,
                    PublicationYear = model.PublicationYear ?? 0,
                    Publisher = model.Publisher,
                    Description = model.Description,
                    ImageUrl = imageUrl
                };

                await _bookService.CreateBookAsync(createDto);
                TempData["Success"] = "Kitap başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            catch (DuplicateException)
            {
                ModelState.AddModelError("ISBN", "Bu ISBN numarası başka bir kitapta kayıtlı.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Hata: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bookDto = await _bookService.GetBookByIdAsync(id);
            if (bookDto == null) return NotFound();

            var model = new BookEditViewModel
            {
                Id = bookDto.Id,
                Title = bookDto.Title,
                Author = bookDto.Author,
                ISBN = bookDto.ISBN,
                Category = bookDto.Category,
                PageCount = bookDto.PageCount ?? 0,
                PublicationYear = bookDto.PublicationYear ?? 0,
                Publisher = bookDto.Publisher,
                Description = bookDto.Description,
                ExistingImageUrl = bookDto.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                string? finalImageUrl = model.ExistingImageUrl;
                if (model.ImageFile != null)
                {
                    var extension = Path.GetExtension(model.ImageFile.FileName);
                    var newImageName = Guid.NewGuid() + extension;
                    var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/books");

                    if (!Directory.Exists(location)) Directory.CreateDirectory(location);

                    var fullPath = Path.Combine(location, newImageName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }
                    finalImageUrl = "/img/books/" + newImageName;
                }

                var updateDto = new UpdateBookDto
                {
                    Title = model.Title,
                    Author = model.Author,
                    ISBN = model.ISBN,
                    Category = model.Category,
                    PageCount = model.PageCount,
                    PublicationYear = model.PublicationYear,
                    Publisher = model.Publisher,
                    Description = model.Description,
                    Language = "Türkçe", // Varsayılan veya modelden gelebilir
                    ImageUrl = finalImageUrl
                };

                await _bookService.UpdateBookAsync(model.Id, updateDto);
                TempData["Success"] = "Kitap başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (DuplicateException)
            {
                ModelState.AddModelError("ISBN", "Bu ISBN numarası başka bir kitapta kayıtlı.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Hata: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _bookService.DeleteBookAsync(id);
                TempData["Success"] = "Kitap ve tüm kopyaları başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Silme hatası: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> EditCopy(int id)
        {
            var copy = await _copyService.GetCopyByIdAsync(id);
            if (copy == null) return NotFound();

            // DTO'da Id olmadığı için, View'a ID'leri ViewBag ile taşıyoruz
            ViewBag.CopyId = copy.Id;
            ViewBag.BookId = copy.BookId;
            ViewBag.CopyNumber = copy.CopyNumber;

            // Formu dolduracak veriler
            var model = new UpdateCopyDto
            {
                ShelfLocation = copy.ShelfLocation,
                Condition = copy.Condition,
                Status = copy.Status
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCopy(int id, UpdateCopyDto model)
        {
            if (!ModelState.IsValid)
            {
               
                return View(model);
            }

            try
            {
             
                var result = await _copyService.UpdateCopyAsync(id, model);

                TempData["Success"] = "Kopya durumu güncellendi.";

                return RedirectToAction("Details", new { id = result.BookId });
            }
            catch (Exception ex)
            {
               
                ModelState.AddModelError("", ex.Message);

            
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCopies(int bookId, int count, string shelfLocation)
        {
            try
            {
                if (count <= 0) throw new Exception("En az 1 kopya eklemelisiniz.");
                if (string.IsNullOrWhiteSpace(shelfLocation)) throw new Exception("Raf konumu belirtmelisiniz.");

                await _copyService.AddCopiesAsync(bookId, count, shelfLocation);
                TempData["Success"] = $"{count} adet kopya başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kopya eklenirken hata: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetCopies(int bookId)
        {
            var copies = await _copyService.GetCopiesByBookIdAsync(bookId);
            return Json(copies);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCopy(int copyId)
        {
            try
            {
                await _copyService.DeleteCopyAsync(copyId);
                return Json(new { success = true, message = "Kopya başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            var copies = await _copyService.GetCopiesByBookIdAsync(id);

            var model = new BookAdminDetailsViewModel
            {
                Book = book,
                Copies = copies
            };
            return View(model);
        }
    }
}