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
    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllAsync();

        return View(books);
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