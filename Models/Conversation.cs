using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RVAS_BookExchange.Models;

public class Conversation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string BookListingId { get; set; } = string.Empty;

    public string BookTitle { get; set; } = string.Empty;

    public string OwnerUserId { get; set; } = string.Empty;

    public string RequesterUserId { get; set; } = string.Empty;

    public List<Message> Messages { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}