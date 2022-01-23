namespace MeetingScheduler.Core.Models.Entities
{
    public class Meeting
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Owner { get; set; } = string.Empty;
        public Guid MeetingId { get; set; } = new Guid();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
