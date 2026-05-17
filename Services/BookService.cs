using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RVAS_BookExchange.Models;
using RVAS_BookExchange.Settings;
using MongoDB.Bson;

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

    public async Task<(List<BookListing> Books, long TotalCount)> SearchAsync(string? title,string? author,string? genre,string? city,int page,int pageSize)
    {
        var filterBuilder = Builders<BookListing>.Filter;

        var filter = filterBuilder.Eq(book => book.IsAvailable, true);

        if (!string.IsNullOrWhiteSpace(title))
        {
            filter &= filterBuilder.Regex(
                book => book.Title,
                new BsonRegularExpression(title, "i")
            );
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            filter &= filterBuilder.Regex(
                book => book.Author,
                new BsonRegularExpression(author, "i")
            );
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            filter &= filterBuilder.Eq(book => book.Genre, genre);
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            filter &= filterBuilder.Eq(book => book.City, city);
        }

        var totalCount = await _books.CountDocumentsAsync(filter);

        var books = await _books
            .Find(filter)
            .SortByDescending(book => book.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (books, totalCount);
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