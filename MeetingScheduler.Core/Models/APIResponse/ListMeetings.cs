using MeetingScheduler.Core.Models.DTO;

namespace MeetingScheduler.Core.Models.APIResponse
{
    public class ListMeetings
    {
        public IList<MeetingDTO>? Meetings { get; set; }
    }
}
