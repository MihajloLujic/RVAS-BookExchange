using RVAS_BookExchange.Models;

namespace RVAS_BookExchange.ViewModels;

public class BookListViewModel
{
    public List<BookListing> Books { get; set; } = new();

    public string? Title { get; set; }

    public string? Author { get; set; }

    public string? Genre { get; set; }

    public string? City { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int PageSize { get; set; } = 12;

    public long TotalBooks { get; set; }
}