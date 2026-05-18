using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RVAS_BookExchange.Models;
using RVAS_BookExchange.Settings;

namespace RVAS_BookExchange.Services;

public class MessageService
{
    private readonly IMongoCollection<Conversation> _conversations;

    public MessageService(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _conversations = database.GetCollection<Conversation>(settings.Value.ConversationsCollectionName);
    }

    public async Task<List<Conversation>> GetUserConversationsAsync(string userId)
    {
        return await _conversations
            .Find(conversation =>
                conversation.OwnerUserId == userId ||
                conversation.RequesterUserId == userId)
            .SortByDescending(conversation => conversation.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Conversation?> GetByIdAsync(string id)
    {
        return await _conversations
            .Find(conversation => conversation.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Conversation?> GetByBookAndUsersAsync(
        string bookListingId,
        string ownerUserId,
        string requesterUserId)
    {
        return await _conversations
            .Find(conversation =>
                conversation.BookListingId == bookListingId &&
                conversation.OwnerUserId == ownerUserId &&
                conversation.RequesterUserId == requesterUserId)
            .FirstOrDefaultAsync();
    }

    public async Task<Conversation> CreateAsync(Conversation conversation)
    {
        await _conversations.InsertOneAsync(conversation);

        return conversation;
    }

    public async Task AddMessageAsync(string conversationId, Message message)
    {
        var update = Builders<Conversation>.Update
            .Push(conversation => conversation.Messages, message)
            .Set(conversation => conversation.UpdatedAt, DateTime.UtcNow);

        await _conversations.UpdateOneAsync(
            conversation => conversation.Id == conversationId,
            update
        );
    }
}