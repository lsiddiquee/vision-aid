namespace VisionAid.Api.Models;

public class ChatResponse
{
    public required string Message { get; set; }
    public required TimeSpan Duration { get; set; }
}