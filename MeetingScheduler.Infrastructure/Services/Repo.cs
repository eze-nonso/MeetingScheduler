using MeetingScheduler.Core.Interfaces;
using MeetingScheduler.Core.Models.Entities;
using MeetingScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Infrastructure.Services
{
    public class Repo : IRepo
    {
        public async Task<IList<Meeting>> ListMeetings(DateTime start, DateTime end)
        {
            return await _context.Meetings
                .Where(meeting => meeting.Start >= start && meeting.End <= end)
                .ToListAsync();
        }

        public async Task<IList<Meeting>> ListMeetings()
        {
            return await _context.Meetings.ToListAsync();
        }

        public async Task CreateMeeting(Meeting newMeeting)
        {
            _context.Meetings.Add(newMeeting);
            await _context.SaveChangesAsync();
        }

        public Repo(MeetingContext context)
        {
            _context = context;
        }
        private readonly MeetingContext _context;
    }
}
