using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RVAS_BookExchange.Models;
using RVAS_BookExchange.Services;
using RVAS_BookExchange.ViewModels;

namespace RVAS_BookExchange.Controllers;

public class BooksController : Controller
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? title,string? author,string? genre,string? city,int page = 1)
    {
        const int pageSize = 12;

        if (page < 1)
        {
            page = 1;
        }

        var result = await _bookService.SearchAsync(
            title,
            author,
            genre,
            city,
            page,
            pageSize
        );

        var totalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize);

        var model = new BookListViewModel
        {
            Books = result.Books,
            Title = title,
            Author = author,
            Genre = genre,
            City = city,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize,
            TotalBooks = result.TotalCount
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest();
        }

        var book = await _bookService.GetByIdAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new BookCreateViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (userId == null || userEmail == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var imagePath = await SaveImageAsync(model.Image);

        var book = new BookListing
        {
            Title = model.Title,
            Author = model.Author,
            Genre = model.Genre,
            Condition = model.Condition,
            Description = model.Description,
            ConditionDescription = model.ConditionDescription,
            City = model.City,
            ImagePath = imagePath,
            OwnerUserId = userId,
            OwnerEmail = userEmail,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsAvailable = true
        };

        await _bookService.CreateAsync(book);

        return RedirectToAction("Index");
    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest();
        }

        var book = await _bookService.GetByIdAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (book.OwnerUserId != userId)
        {
            return Forbid();
        }

        var model = new BookEditViewModel
        {
            Id = book.Id!,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Condition = book.Condition,
            Description = book.Description,
            ConditionDescription = book.ConditionDescription,
            City = book.City,
            ExistingImagePath = book.ImagePath
        };

        return View(model);
    }
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existingBook = await _bookService.GetByIdAsync(model.Id);

        if (existingBook == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (existingBook.OwnerUserId != userId)
        {
            return Forbid();
        }

        string? imagePath = existingBook.ImagePath;

        if (model.NewImage != null && model.NewImage.Length > 0)
        {
            imagePath = await SaveImageAsync(model.NewImage);
        }

        existingBook.Title = model.Title;
        existingBook.Author = model.Author;
        existingBook.Genre = model.Genre;
        existingBook.Condition = model.Condition;
        existingBook.Description = model.Description;
        existingBook.ConditionDescription = model.ConditionDescription;
        existingBook.City = model.City;
        existingBook.ImagePath = imagePath;
        existingBook.UpdatedAt = DateTime.UtcNow;

        await _bookService.UpdateAsync(existingBook.Id!, existingBook);

        return RedirectToAction("Index", "MyListings");
    }
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        await _bookService.DeleteAsync(id, userId);

        return RedirectToAction("Index", "MyListings");
    }

    private async Task<string?> SaveImageAsync(IFormFile? image)
    {
        if (image == null || image.Length == 0)
        {
            return null;
        }

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads"
        );

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileExtension = Path.GetExtension(image.FileName);
        var fileName = Guid.NewGuid().ToString() + fileExtension;
        var filePath = Path.Combine(uploadsFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(stream);

        return "/uploads/" + fileName;
    }
}