using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RVAS_BookExchange.Services;

namespace RVAS_BookExchange.Controllers;

[Authorize]
public class MyListingsController : Controller
{
    private readonly BookService _bookService;

    public MyListingsController(BookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var myBooks = await _bookService.GetByOwnerAsync(userId);

        return View(myBooks);
    }
}