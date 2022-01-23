namespace MeetingScheduler.Core.Models.DTO;

public class MeetingDTO
{
    public string Start { get; set; } = string.Empty;
    public string End { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public Guid MeetingId { get; set; }
}
