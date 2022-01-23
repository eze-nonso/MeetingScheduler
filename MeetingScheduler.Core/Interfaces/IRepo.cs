using MeetingScheduler.Core.Models.Entities;

namespace MeetingScheduler.Core.Interfaces
{
    public interface IRepo
    {
        public Task<IList<Meeting>> ListMeetings(DateTime start, DateTime end);
        public Task<IList<Meeting>> ListMeetings();
        public Task CreateMeeting(Meeting newMeeting);
    }
}
