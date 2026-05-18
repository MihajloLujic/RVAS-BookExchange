using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RVAS_BookExchange.Models;

public class BookRequest
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string BookListingId { get; set; } = string.Empty;

    public string BookTitle { get; set; } = string.Empty;

    public string OwnerUserId { get; set; } = string.Empty;

    public string RequesterUserId { get; set; } = string.Empty;

    public string RequesterEmail { get; set; } = string.Empty;

    public string RequestType { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public string? ResponseComment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RespondedAt { get; set; }
}