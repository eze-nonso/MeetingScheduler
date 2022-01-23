namespace MeetingScheduler.Core.Models;

[Serializable]
public class ConflictTimeException : Exception
{
    public ConflictTimeException(string? message) : base(message)
    {
    }
}
