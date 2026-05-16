using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RVAS_BookExchange.Models;
using RVAS_BookExchange.Settings;

namespace RVAS_BookExchange.Services;

public class UserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _users = database.GetCollection<User>(settings.Value.UsersCollectionName);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }
}