using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RVAS_BookExchange.Models;
using RVAS_BookExchange.Services;

namespace RVAS_BookExchange.Controllers;

[Authorize]
public class RequestsController : Controller
{
    private readonly RequestService _requestService;
    private readonly BookService _bookService;

    public RequestsController(RequestService requestService, BookService bookService)
    {
        _requestService = requestService;
        _bookService = bookService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string bookId, string requestType)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);

        if (currentUserId == null || currentUserEmail == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (string.IsNullOrWhiteSpace(bookId))
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(requestType))
        {
            ModelState.AddModelError("", "Tip zahteva je obavezan.");
            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        var book = await _bookService.GetByIdAsync(bookId);

        if (book == null)
        {
            return NotFound();
        }

        if (book.OwnerUserId == currentUserId)
        {
            return BadRequest("Ne možete poslati zahtev za svoju knjigu.");
        }

        var request = new BookRequest
        {
            BookListingId = book.Id!,
            BookTitle = book.Title,
            OwnerUserId = book.OwnerUserId,
            RequesterUserId = currentUserId,
            RequesterEmail = currentUserEmail,
            RequestType = requestType,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _requestService.CreateAsync(request);

        return RedirectToAction("Details", "Books", new { id = bookId });
    }

    [HttpGet]
    public async Task<IActionResult> Incoming()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var requests = await _requestService.GetIncomingForOwnerAsync(currentUserId);

        return View(requests);
    }

    [HttpGet]
    public async Task<IActionResult> Sent()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var requests = await _requestService.GetSentByUserAsync(currentUserId);

        return View(requests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(string id, string? responseComment)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        await _requestService.RespondAsync(id, currentUserId, "Accepted", responseComment);

        return RedirectToAction("Incoming");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(string id, string? responseComment)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        await _requestService.RespondAsync(id, currentUserId, "Rejected", responseComment);

        return RedirectToAction("Incoming");
    }
}