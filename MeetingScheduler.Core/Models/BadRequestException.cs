namespace MeetingScheduler.Core.Models;

[Serializable]
public class BadRequestException : Exception
{
    public BadRequestException(string? message) : base(message)
    {
    }
}
