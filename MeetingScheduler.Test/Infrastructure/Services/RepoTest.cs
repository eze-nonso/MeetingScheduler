using MeetingScheduler.Core.Models.Constants;
using MeetingScheduler.Core.Models.Entities;
using MeetingScheduler.Core.Services;
using MeetingScheduler.Infrastructure.Data;
using MeetingScheduler.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MeetingScheduler.Test.Infrastructure.Services;

public class RepoTest
{
    private readonly Repo repo;
    private readonly MeetingContext inMemoryContext;
    public RepoTest()
    {
        AppSettings.MaxMeetingHours = 2;
        AppSettings.MinMeetingHours = 0.5;
        AppSettings.EndOfDayHour = 17;
        AppSettings.StartOfDayHour = 9;
        AppSettings.DateStringFormat = "yyyy-MM-ddTHH:mm:ssZ";
        var options = new DbContextOptionsBuilder<MeetingContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        inMemoryContext = new MeetingContext(options);

        repo = new Repo(inMemoryContext);
    }

    private const string guidString = "1BAC5D4E-C4F8-4271-B013-90D307F24F31";

    private const int tenAM = 10;
    private const int elevenAM = 11;

    private void Dispose()
    {
        ((IDisposable)inMemoryContext).Dispose();
    }

    [Fact]
    [Trait("Repo", "ListMeetings")]
    public async Task Should_Return_Meeting_List()
    {
        DateTime now = DateTime.Now;
        string start = new DateTime(now.Year, now.Month, now.Day, tenAM, 0, 0).ToString(AppSettings.DateStringFormat);
        string end = new DateTime(now.Year, now.Month, now.Day, elevenAM, 0, 0).ToString(AppSettings.DateStringFormat);

        DateTime startDate = IntervalUtil.Parse(start);
        DateTime endDate = IntervalUtil.Parse(end);
        DateTime startOfDay = startDate.AddHours(-tenAM);
        DateTime endOfDay = startDate.AddHours(12).AddMinutes(59).AddSeconds(59);
        Meeting meeting = new Meeting()
        {
            CreatedAt = DateTime.Now,
            MeetingId = new Guid(guidString),
            Start = startDate,
            End = endDate
        };

        inMemoryContext.Meetings.Add(meeting);
        inMemoryContext.SaveChanges();

        var result = await repo.ListMeetings(startOfDay, endOfDay);
        Assert.Single(result);
        Assert.Equal(meeting.MeetingId, result[0].MeetingId);
    }

    [Fact]
    [Trait("Repo", "ListMeetings")]
    public async Task ListMeetings_Without_Param_Should_Return_Meeting_List()
    {
        DateTime now = DateTime.Now;
        string start = new DateTime(now.Year, now.Month, now.Day, tenAM, 0, 0).ToString(AppSettings.DateStringFormat);
        string end = new DateTime(now.Year, now.Month, now.Day, elevenAM, 0, 0).ToString(AppSettings.DateStringFormat);

        DateTime startDate = IntervalUtil.Parse(start);
        DateTime endDate = IntervalUtil.Parse(end);
        Meeting meeting = new()
        {
            CreatedAt = DateTime.Now,
            MeetingId = new Guid(guidString),
            Start = startDate,
            End = endDate
        };

        inMemoryContext.Meetings.Add(meeting);
        inMemoryContext.SaveChanges();

        var result = await repo.ListMeetings();
        Assert.Single(result);
        Assert.Equal(meeting.MeetingId, result[0].MeetingId);
    }

    [Fact]
    [Trait("Repo", "CreateMeeting")]
    public async Task Should_Create_New_Meeting()
    {
        DateTime now = DateTime.Now;
        string start = new DateTime(now.Year, now.Month, now.Day, tenAM, 0, 0).ToString(AppSettings.DateStringFormat);
        string end = new DateTime(now.Year, now.Month, now.Day, elevenAM, 0, 0).ToString(AppSettings.DateStringFormat);

        DateTime startDate = IntervalUtil.Parse(start);
        DateTime endDate = IntervalUtil.Parse(end);
        Meeting meeting = new()
        {
            CreatedAt = DateTime.Now,
            MeetingId = new Guid(guidString),
            Start = startDate,
            End = endDate
        };

        await repo.CreateMeeting(meeting);
        var result = inMemoryContext.Meetings
            .Where(m => m.MeetingId == meeting.MeetingId)
            .ToList();
        Assert.Single(result);
    }


}
