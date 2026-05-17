using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RVAS_BookExchange.Models;
using RVAS_BookExchange.Settings;

namespace RVAS_BookExchange.Services;

public class BookService
{
    private readonly IMongoCollection<BookListing> _books;

    public BookService(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _books = database.GetCollection<BookListing>(settings.Value.BooksCollectionName);
    }

    public async Task CreateAsync(BookListing book)
    {
        await _books.InsertOneAsync(book);
    }

    public async Task<BookListing?> GetByIdAsync(string id)
    {
        return await _books.Find(book => book.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<BookListing>> GetAllAsync()
    {
        return await _books
            .Find(book => book.IsAvailable == true)
            .SortByDescending(book => book.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<BookListing>> GetByOwnerAsync(string ownerUserId)
    {
        return await _books
            .Find(book => book.OwnerUserId == ownerUserId)
            .SortByDescending(book => book.CreatedAt)
            .ToListAsync();
    }
    public async Task UpdateAsync(string id, BookListing updatedBook)
    {
        updatedBook.UpdatedAt = DateTime.UtcNow;

        await _books.ReplaceOneAsync(book => book.Id == id, updatedBook);
    }

    public async Task DeleteAsync(string id, string ownerUserId)
    {
        await _books.DeleteOneAsync(book => book.Id == id && book.OwnerUserId == ownerUserId);
    }
}