using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RVAS_BookExchange.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public List<string> FavoriteGenres { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
