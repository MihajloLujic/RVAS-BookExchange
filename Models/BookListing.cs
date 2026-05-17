using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RVAS_BookExchange.Models;

public class BookListing
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public string Condition { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ConditionDescription { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string? ImagePath { get; set; }

    public string OwnerUserId { get; set; } = string.Empty;

    public string OwnerEmail { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}