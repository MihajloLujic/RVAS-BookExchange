using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RVAS_BookExchange.Models;
using RVAS_BookExchange.Services;

namespace RVAS_BookExchange.Controllers;

[Authorize]
public class MessagesController : Controller
{
    private readonly MessageService _messageService;
    private readonly BookService _bookService;

    public MessagesController(MessageService messageService, BookService bookService)
    {
        _messageService = messageService;
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var conversations = await _messageService.GetUserConversationsAsync(currentUserId);

        return View(conversations);
    }

    [HttpGet]
    public async Task<IActionResult> Thread(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest();
        }

        var conversation = await _messageService.GetByIdAsync(id);

        if (conversation == null)
        {
            return NotFound();
        }

        if (conversation.OwnerUserId != currentUserId &&
            conversation.RequesterUserId != currentUserId)
        {
            return Forbid();
        }

        return View(conversation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(string bookId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (string.IsNullOrWhiteSpace(bookId))
        {
            return BadRequest();
        }

        var book = await _bookService.GetByIdAsync(bookId);

        if (book == null)
        {
            return NotFound();
        }

        if (book.OwnerUserId == currentUserId)
        {
            return BadRequest("Ne možete započeti razgovor za sopstvenu knjigu.");
        }

        var existingConversation = await _messageService.GetByBookAndUsersAsync(
            book.Id!,
            book.OwnerUserId,
            currentUserId
        );

        if (existingConversation != null)
        {
            return RedirectToAction("Thread", new { id = existingConversation.Id });
        }

        var conversation = new Conversation
        {
            BookListingId = book.Id!,
            BookTitle = book.Title,
            OwnerUserId = book.OwnerUserId,
            RequesterUserId = currentUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _messageService.CreateAsync(conversation);

        return RedirectToAction("Thread", new { id = conversation.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(string conversationId, string text)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);

        if (currentUserId == null || currentUserEmail == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (string.IsNullOrWhiteSpace(conversationId))
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return RedirectToAction("Thread", new { id = conversationId });
        }

        var conversation = await _messageService.GetByIdAsync(conversationId);

        if (conversation == null)
        {
            return NotFound();
        }

        if (conversation.OwnerUserId != currentUserId &&
            conversation.RequesterUserId != currentUserId)
        {
            return Forbid();
        }

        var message = new Message
        {
            SenderUserId = currentUserId,
            SenderEmail = currentUserEmail,
            Text = text,
            SentAt = DateTime.UtcNow
        };

        await _messageService.AddMessageAsync(conversationId, message);

        return RedirectToAction("Thread", new { id = conversationId });
    }
}