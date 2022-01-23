using MeetingScheduler.Core.Models;
using MeetingScheduler.Core.Models.DTO;

namespace MeetingScheduler.Core.Interfaces;

public interface IMeetingService
{
    public Task<IList<MeetingDTO>> ListMeetings(DateRangeParam dateRange);
    public Task<string> CreateMeeting(NewMeetingReq newMeeting);
    public Task<IList<MeetingIntervalDTO>> ListAvailableSlots(DateRangeParam timeRange);
}
