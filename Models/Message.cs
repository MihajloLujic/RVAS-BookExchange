namespace RVAS_BookExchange.Models;

public class Message
{
    public string SenderUserId { get; set; } = string.Empty;

    public string SenderEmail { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}