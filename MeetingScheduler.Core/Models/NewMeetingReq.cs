namespace MeetingScheduler.Core.Models;

public class NewMeetingReq
{
    public string Start { get; set; } = string.Empty;
    public string End { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
}
