using MeetingScheduler.API.Controllers;
using MeetingScheduler.Core.Interfaces;
using MeetingScheduler.Core.Interfaces.Helpers;
using MeetingScheduler.Core.Models;
using MeetingScheduler.Core.Models.APIResponse;
using MeetingScheduler.Core.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MeetingScheduler.Test.API;

public class MeetingControllerTest
{
    private readonly Mock<IMeetingService> meetingServiceMock;
    private readonly Mock<ILoggerService> loggerServiceMock;
    private readonly IMeetingService meetingService;
    private readonly ILoggerService loggerService;
    private readonly MeetingController sut;
    public MeetingControllerTest()
    {
        meetingServiceMock = new Mock<IMeetingService>();
        meetingService = meetingServiceMock.Object;
        loggerServiceMock = new Mock<ILoggerService>();
        loggerService = loggerServiceMock.Object;

        sut = new MeetingController(
            _loggerService: loggerService, _meetingService: meetingService
            );
    }

    [Fact]
    [Trait("MeetingController", "ListMeetings")]
    public async Task Should_Return_MeetingListDTO()
    {
        const string guidString = "1BAC5D4E-C4F8-4271-B013-90D307F24F31";
        IList<MeetingDTO> meetings = new List<MeetingDTO>
        {
            new MeetingDTO()
            {
                Start = "2021-07-05T09:00:00Z",
                End = "2021-07-05T10:00:00Z",
                Owner = "Odogwu",
                MeetingId = new Guid(guidString)
            }
        };
        meetingServiceMock
            .Setup(x => x.ListMeetings(It.IsAny<DateRangeParam>()).Result)
            .Returns(meetings);

        IActionResult response = await sut.ListMeetings(null, null);
        OkObjectResult? okObject = response as OkObjectResult;
        Assert.NotNull(okObject?.Value);
        var meetingRes = Assert.IsAssignableFrom<ListMeetings>(okObject?.Value);

        Assert.Equal(meetings, meetingRes.Meetings);
    }

    [Fact]
    [Trait("MeetingController", "ListMeetings")]
    public async Task Should_Call_Logger_With_500StatusCode()
    {
        meetingServiceMock
            .Setup(x => x.ListMeetings(It.IsAny<DateRangeParam>()).Result)
            .Throws<Exception>();

        IActionResult response = await sut.ListMeetings(null, null);
        loggerServiceMock.Verify(mock => mock.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.AtLeastOnce());
        ObjectResult? badRequest = response as ObjectResult;
        Assert.NotNull(badRequest?.Value);
        Assert.Equal(500, badRequest?.StatusCode);
    }

    [Fact]
    [Trait("MeetingController", "CreateMeeting")]
    public async Task Should_Create_Meeting_AndReturn_Id()
    {
        const string guidString = "1BAC5D4E-C4F8-4271-B013-90D307F24F31";
        NewMeetingReq newMeeting = new()
        {
            Start = "2021-07-05T09:00:00Z",
            End = "2021-07-05T10:00:00Z",
            Owner = "Odogwu"
        };
        meetingServiceMock
            .Setup(x => x.CreateMeeting(It.IsAny<NewMeetingReq>()).Result)
            .Returns(guidString);

        IActionResult response = await sut.CreateMeeting(newMeeting);
        OkObjectResult? okObject = response as OkObjectResult;
        Assert.NotNull(okObject?.Value);
        var meetingRes = Assert.IsAssignableFrom<NewMeeting>(okObject?.Value);

        Assert.Equal(guidString, meetingRes.MeetingId);
    }

    [Fact]
    [Trait("MeetingController", "CreateMeeting")]
    public async Task Create_Fail_Should_Call_Logger_With_500StatusCode()
    {
        NewMeetingReq newMeeting = new()
        {
            Start = "2021-07-05T09:00:00Z",
            End = "2021-07-05T10:00:00Z",
            Owner = "Odogwu"
        };
        meetingServiceMock
            .Setup(x => x.CreateMeeting(It.IsAny<NewMeetingReq>()).Result)
            .Throws(new());

        IActionResult response = await sut.CreateMeeting(newMeeting);
        loggerServiceMock.Verify(mock => mock.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.AtLeastOnce());
        ObjectResult? badRequest = response as ObjectResult;
        Assert.NotNull(badRequest?.Value);
        Assert.Equal(500, badRequest?.StatusCode);
    }

    [Fact]
    [Trait("MeetingController", "CreateMeeting")]
    public async Task Create_Fail_Should_Call_Logger_With_400StatusCode_On_BadRequestException()
    {
        NewMeetingReq newMeeting = new()
        {
            Start = "2021-07-05T09:00:00Z",
            End = "2021-07-05T10:00:00Z",
            Owner = "Odogwu"
        };
        meetingServiceMock
            .Setup(x => x.CreateMeeting(It.IsAny<NewMeetingReq>()).Result)
            .Throws(new BadRequestException(""));

        IActionResult response = await sut.CreateMeeting(newMeeting);
        loggerServiceMock.Verify(mock => mock.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.AtLeastOnce());
        ObjectResult? badRequest = response as ObjectResult;
        Assert.NotNull(badRequest?.Value);
        Assert.Equal(400, badRequest?.StatusCode);
    }

    [Fact]
    [Trait("MeetingController", "CreateMeeting")]
    public async Task Create_Fail_Should_Call_Logger_With_400StatusCode_On_ConflictTimeException()
    {
        NewMeetingReq newMeeting = new()
        {
            Start = "2021-07-05T09:00:00Z",
            End = "2021-07-05T10:00:00Z",
            Owner = "Odogwu"
        };
        meetingServiceMock
            .Setup(x => x.CreateMeeting(It.IsAny<NewMeetingReq>()).Result)
            .Throws(new ConflictTimeException(""));

        IActionResult response = await sut.CreateMeeting(newMeeting);
        loggerServiceMock.Verify(mock => mock.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.AtLeastOnce());
        ObjectResult? badRequest = response as ObjectResult;
        Assert.NotNull(badRequest?.Value);
        Assert.Equal(400, badRequest?.StatusCode);
    }

    [Fact]
    [Trait("MeetingController", "ListAvailableSlots")]
    public async Task Should_Return_Available_slots()
    {
        IList<MeetingIntervalDTO> timeSlots = new List<MeetingIntervalDTO>
        {
            new MeetingIntervalDTO()
            {
                Start = "2021-07-05T09:00:00Z",
                End = "2021-07-05T10:00:00Z"
            }
        };
        meetingServiceMock
            .Setup(x => x.ListAvailableSlots(It.IsAny<DateRangeParam>()).Result)
            .Returns(timeSlots);

        IActionResult response = await sut.ListAvailableSlots(null, null);
        OkObjectResult? okObject = response as OkObjectResult;
        Assert.NotNull(okObject?.Value);
        var availableSlotsRes = Assert.IsAssignableFrom<AvailableTimeSlots>(okObject?.Value);

        Assert.Equal(timeSlots, availableSlotsRes.AvailableSlots);
    }

    [Fact]
    [Trait("MeetingController", "ListAvailableSlots")]
    public async Task Should_Call_Logger_With_400Status_On_ConflictTimeException()
    {
        meetingServiceMock
            .Setup(x => x.ListAvailableSlots(It.IsAny<DateRangeParam>()).Result)
            .Throws(new ConflictTimeException(""));

        IActionResult response = await sut.ListAvailableSlots(null, null);
        loggerServiceMock.Verify(mock => mock.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.AtLeastOnce());
        ObjectResult? badRequest = response as ObjectResult;
        Assert.NotNull(badRequest?.Value);
        Assert.Equal(400, badRequest?.StatusCode);
    }

    [Fact]
    [Trait("MeetingController", "ListAvailableSlots")]
    public async Task Should_Call_Logger_With_400Status_On_BadRequestException()
    {
        meetingServiceMock
            .Setup(x => x.ListAvailableSlots(It.IsAny<DateRangeParam>()).Result)
            .Throws(new BadRequestException(""));

        IActionResult response = await sut.ListAvailableSlots(null, null);
        loggerServiceMock.Verify(mock => mock.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.AtLeastOnce());
        ObjectResult? badRequest = response as ObjectResult;
        Assert.NotNull(badRequest?.Value);
        Assert.Equal(400, badRequest?.StatusCode);
    }

    [Fact]
    [Trait("MeetingController", "ListAvailableSlots")]
    public async Task Should_Call_Logger_With_500StatusReturn()
    {
        meetingServiceMock
            .Setup(x => x.ListAvailableSlots(It.IsAny<DateRangeParam>()).Result)
            .Throws(new(""));

        IActionResult response = await sut.ListAvailableSlots(null, null);
        loggerServiceMock.Verify(mock => mock.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.AtLeastOnce());
        ObjectResult? badRequest = response as ObjectResult;
        Assert.NotNull(badRequest?.Value);
        Assert.Equal(500, badRequest?.StatusCode);
    }
}
