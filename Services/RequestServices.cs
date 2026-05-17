using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RVAS_BookExchange.Models;
using RVAS_BookExchange.Settings;

namespace RVAS_BookExchange.Services;

public class RequestService
{
    private readonly IMongoCollection<BookRequest> _requests;

    public RequestService(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _requests = database.GetCollection<BookRequest>(settings.Value.RequestsCollectionName);
    }

    public async Task CreateAsync(BookRequest request)
    {
        await _requests.InsertOneAsync(request);
    }

    public async Task<BookRequest?> GetByIdAsync(string id)
    {
        return await _requests
            .Find(request => request.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<BookRequest>> GetIncomingForOwnerAsync(string ownerUserId)
    {
        return await _requests
            .Find(request => request.OwnerUserId == ownerUserId)
            .SortByDescending(request => request.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<BookRequest>> GetSentByUserAsync(string requesterUserId)
    {
        return await _requests
            .Find(request => request.RequesterUserId == requesterUserId)
            .SortByDescending(request => request.CreatedAt)
            .ToListAsync();
    }

    public async Task RespondAsync(string requestId, string ownerUserId, string status, string? responseComment)
    {
        var update = Builders<BookRequest>.Update
            .Set(request => request.Status, status)
            .Set(request => request.ResponseComment, responseComment)
            .Set(request => request.RespondedAt, DateTime.UtcNow);

        await _requests.UpdateOneAsync(
            request => request.Id == requestId && request.OwnerUserId == ownerUserId,
            update
        );
    }
}