using AutoMapper;
using MeetingScheduler.Core.Extensions;
using MeetingScheduler.Core.Interfaces;
using MeetingScheduler.Core.Models;
using MeetingScheduler.Core.Models.Constants;
using MeetingScheduler.Core.Models.DTO;
using MeetingScheduler.Core.Models.Entities;
using MeetingScheduler.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MeetingScheduler.Test.Core.Services;

public class MeetingServiceTest
{
    private const int outOfRangeEndTime = 18;
    private const int correctStartTime = 10;
    private const int correctEndTime = 11;
    private const int invalidMinuteTimestamp = 31;
    private const string guidString = "1BAC5D4E-C4F8-4271-B013-90D307F24F31";
    private readonly Mock<IRepo> repoMock;
    private readonly IIntervalUtil intervalUtil;
    private readonly IMeetingService sut;
    public MeetingServiceTest()
    {
        AppSettings.MaxMeetingHours = 2;
        AppSettings.MinMeetingHours = 0.5;
        AppSettings.EndOfDayHour = 17;
        AppSettings.StartOfDayHour = 9;
        AppSettings.DateStringFormat = "yyyy-MM-ddTHH:mm:ssK";
        repoMock = new Mock<IRepo>();
        intervalUtil = new IntervalUtil();
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddProfile<Maps>();
        }

        );
        IMapper mapper = config.CreateMapper();
        AppSettings.MaxMeetingHours = 2;

        sut = new MeetingService(repoMock.Object, intervalUtil, mapper);
    }

    [Trait("MeetingService", "CreateMeeting")]
    [Fact]
    public async Task CreateMeeting_Should_Return_Created_Meeting_Id()
    {
        repoMock.Setup(m => m.ListMeetings().Result).Returns(new List<Meeting>());
        DateTime now = DateTime.Now;
        NewMeetingReq newReq = new()
        {
            Start = new DateTime(now.Year, now.Month, now.Day, AppSettings.EndOfDayHour - 1, 0, 0).ToString(AppSettings.DateStringFormat),
            End = new DateTime(now.Year, now.Month, now.Day, AppSettings.EndOfDayHour, 0, 0).ToString(AppSettings.DateStringFormat),
            Owner = "Odogwu"
        };
        DateTime startDate = IntervalUtil.Parse(newReq.Start);
        DateTime endDate = IntervalUtil.Parse(newReq.End);

        if (endDate > new DateTime(now.Year, now.Month, now.Day, AppSettings.EndOfDayHour, 0, 0))
        {
            // cannot create meeting
            await Assert.ThrowsAnyAsync<Exception>(() => sut.CreateMeeting(newReq));
        }
        else
        {
            var result = await sut.CreateMeeting(newReq);
            // if this test is run after EndOfDayHour(5pm), creating a meeting will fail with error
            // use this check to ensure correct behavior
            if (intervalUtil.IsWithinAllowableRange(startDate, endDate))
            {
                Assert.IsAssignableFrom<string>(result);
                Assert.NotEmpty(result);
            }
        }
    }

    [Trait("MeetingService", "CreateMeeting")]
    [Fact]
    public async Task Should_Error_When_StartDate_After_EndDate()
    {
        repoMock.Setup(m => m.ListMeetings().Result).Returns(new List<Meeting>());
        DateTime now = DateTime.Now;
        NewMeetingReq newReq = new()
        {
            Start = new DateTime(now.Year, now.Month, now.Day, now.Hour + 2, 0, 0).ToString(AppSettings.DateStringFormat),
            End = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0).ToString(AppSettings.DateStringFormat),
            Owner = "Odogwu"
        };
        await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateMeeting(newReq));
    }

    [Trait("MeetingService", "CreateMeeting")]
    [Fact]
    public async Task Should_Error_When_Meeting_Length_Exceeds_Max()
    {
        repoMock.Setup(m => m.ListMeetings().Result).Returns(new List<Meeting>());
        DateTime now = DateTime.Now;
        NewMeetingReq newReq = new()
        {
            Start = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0).ToString(AppSettings.DateStringFormat),
            End = new DateTime(now.Year, now.Month, now.Day, now.Hour + 3, 30, 0).ToString(AppSettings.DateStringFormat),
            Owner = "Odogwu"
        };
        await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateMeeting(newReq));
    }

    [Trait("MeetingService", "CreateMeeting")]
    [Fact]
    public async Task Should_Error_When_Dates_Not_Proper_ISO_Format()
    {
        repoMock.Setup(m => m.ListMeetings().Result).Returns(new List<Meeting>());
        DateTime now = DateTime.Now;
        NewMeetingReq newReq = new()
        {
            Start = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0).ToString(),
            End = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 30, 0).ToString(),
            Owner = "Odogwu"
        };
        await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateMeeting(newReq));
    }

    [Trait("MeetingService", "CreateMeeting")]
    [Fact]
    public async Task Should_Error_When_Dates_OutSide_MaxAndMinRange()
    {
        repoMock.Setup(m => m.ListMeetings().Result).Returns(new List<Meeting>());
        DateTime now = DateTime.Now;
        NewMeetingReq newReq = new()
        {
            Start = new DateTime(now.Year, now.Month, now.Day, outOfRangeEndTime - 2, 0, 0).ToString(AppSettings.DateStringFormat),
            End = new DateTime(now.Year, now.Month, now.Day, outOfRangeEndTime, 0, 0).ToString(AppSettings.DateStringFormat),
            Owner = "Odogwu"
        };
        await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateMeeting(newReq));
    }

    [Trait("MeetingService", "CreateMeeting")]
    [Fact]
    public async Task Should_Error_When_Date_Not_In_30_Or_0_TimeStamp_Format()
    {
        repoMock.Setup(m => m.ListMeetings().Result).Returns(new List<Meeting>());
        DateTime now = DateTime.Now;
        NewMeetingReq newReq = new()
        {
            Start = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0).ToString(AppSettings.DateStringFormat),
            End = new DateTime(now.Year, now.Month, now.Day, now.Hour + 2, invalidMinuteTimestamp, 0).ToString(AppSettings.DateStringFormat),
            Owner = "Odogwu"
        };
        await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateMeeting(newReq));
    }

    [Trait("MeetingService", "CreateMeeting")]
    [Fact]
    public async Task Should_Throw_ConflictTimeException_When_Existing_Meeting_Overlaps()
    {
        const string guidString = "1BAC5D4E-C4F8-4271-B013-90D307F24F31";
        repoMock.Setup(m => m.ListMeetings().Result).Returns(new List<Meeting>());
        DateTime now = DateTime.Now;
        NewMeetingReq newReq = new()
        {
            Start = new DateTime(now.Year, now.Month, now.Day, AppSettings.EndOfDayHour - 1, 0, 0).ToString(AppSettings.DateStringFormat),
            End = new DateTime(now.Year, now.Month, now.Day, AppSettings.EndOfDayHour, 0, 0).ToString(AppSettings.DateStringFormat),
            Owner = "Odogwu"
        };
        DateTime startDate = IntervalUtil.Parse(newReq.Start);
        DateTime endDate = IntervalUtil.Parse(newReq.End);

        if (endDate > new DateTime(now.Year, now.Month, now.Day, AppSettings.EndOfDayHour, 0, 0))
        {
            // cannot create meeting
            await Assert.ThrowsAnyAsync<Exception>(() => sut.CreateMeeting(newReq));
        }
        else
        {
            var result = await sut.CreateMeeting(newReq);

            // if this test is run after EndOfDayHour(5pm)
            // creating a meeting will fail with error
            // use this check to ensure correct behavior
            if (intervalUtil.IsWithinAllowableRange(startDate, endDate))
            {
                repoMock.Setup(m => m.ListMeetings().Result).Returns(new List<Meeting>()
            {
                new Meeting()
                {
                    Start = startDate,
                    End = endDate,
                    Owner = "Odogwu",
                    MeetingId = new Guid(guidString),
                    CreatedAt = DateTime.Now
                }
            });
                Assert.IsAssignableFrom<string>(result);
                Assert.NotEmpty(result);
                await Assert.ThrowsAsync<ConflictTimeException>(() => sut.CreateMeeting(newReq));
            }
        }
    }

    [Trait("MeetingService", "ListMeetings")]
    [Fact]
    public async Task Should_Return_List_Of_Meetings()
    {
        DateTime now = DateTime.Now;
        string start = new DateTime(now.Year, now.Month, now.Day, correctStartTime, 0, 0).ToString(AppSettings.DateStringFormat);
        string end = new DateTime(now.Year, now.Month, now.Day, correctEndTime, 0, 0).ToString(AppSettings.DateStringFormat);

        DateTime startDate = IntervalUtil.Parse(start);
        DateTime endDate = IntervalUtil.Parse(end);
        Meeting newMeeting = new()
        {
            Start = startDate,
            End = endDate,
            Owner = "Odogwu",
            MeetingId = new Guid(guidString),
            CreatedAt = DateTime.Now
        };
        repoMock.Setup(m => m.ListMeetings(It.IsAny<DateTime>(), It.IsAny<DateTime>()).Result).Returns(new List<Meeting>() { newMeeting });
        DateRangeParam dateRange = new(start, end);
        IList<MeetingDTO> meetings = await sut.ListMeetings(dateRange);
        Assert.Single(meetings);
        Assert.Equal(newMeeting.MeetingId, meetings[0].MeetingId);
    }

    [Trait("MeetingService", "ListAvailableSlots")]
    [Fact]
    public async Task Should_Return_List_Of_Available_Meeting_Slots()
    {
        DateTime now = DateTime.Now;
        string start = new DateTime(now.Year, now.Month, now.Day, AppSettings.StartOfDayHour, 0, 0).ToString(AppSettings.DateStringFormat);
        string end = new DateTime(now.Year, now.Month, now.Day, AppSettings.EndOfDayHour, 0, 0).ToString(AppSettings.DateStringFormat);
        DateTime startDate = IntervalUtil.Parse(start);
        DateTime endDate = IntervalUtil.Parse(end);
        int intervalsBetweenStartAndEnd = Convert.ToInt32((endDate - startDate).TotalHours / AppSettings.MinMeetingHours);
        int adjustedIntervals;
        if (intervalsBetweenStartAndEnd > 0)
        {
            if (now > startDate)
            {
                if (now < endDate)
                {
                    adjustedIntervals = (int)(Math.Floor((endDate - now).TotalHours / (endDate - startDate).TotalHours * intervalsBetweenStartAndEnd));
                }
                else
                {
                    adjustedIntervals = 0;
                }
            }
            else
            {
                adjustedIntervals = intervalsBetweenStartAndEnd;
            }
        }
        else { adjustedIntervals = intervalsBetweenStartAndEnd; }

        repoMock.Setup(m => m.ListMeetings().Result).Returns(new List<Meeting>());

        DateRangeParam dateRange = new(start, end);
        IList<MeetingIntervalDTO> meetingIntervals = await sut.ListAvailableSlots(dateRange);
        Assert.Equal(adjustedIntervals, meetingIntervals.Count);
    }

}
