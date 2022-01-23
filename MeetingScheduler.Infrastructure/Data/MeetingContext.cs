using MeetingScheduler.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Infrastructure.Data
{
    public class MeetingContext : DbContext
    {
        public MeetingContext(DbContextOptions<MeetingContext> options)
            : base(options)
        {

        }
        public DbSet<Meeting> Meetings { get; set; } = null!;
    }
}
