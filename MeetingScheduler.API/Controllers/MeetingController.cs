using MeetingScheduler.Core.Interfaces;
using MeetingScheduler.Core.Interfaces.Helpers;
using MeetingScheduler.Core.Models;
using MeetingScheduler.Core.Models.APIResponse;
using MeetingScheduler.Core.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.API.Controllers;

[Route("api/meetingscheduler")]
[ApiController]
public class MeetingController : ControllerBase
{
    public MeetingController(ILoggerService _loggerService, IMeetingService _meetingService)
    {
        loggerService = _loggerService;
        meetingService = _meetingService;
    }
    private readonly ILoggerService loggerService;
    private readonly IMeetingService meetingService;
    // GET: MeetingController
    [HttpGet("meetings")]
    public async Task<IActionResult> ListMeetings(string? start, string? end)
    {
        try
        {
            IList<MeetingDTO> meetings = await meetingService.ListMeetings(new(start, end));
            return Ok(
            new ListMeetings()
            {   
                Meetings = meetings
            });

        }
        catch (Exception ex)
        {
            loggerService.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    [HttpPost("meetings")]
    public async Task<IActionResult> CreateMeeting([FromBody] NewMeetingReq newMeeting)
    {
        try
        {
            string meetingId = await meetingService.CreateMeeting(newMeeting);
            return Ok(
            new NewMeeting()
            {
                MeetingId = meetingId
            });

        }
        catch (Exception ex) when (ex is ConflictTimeException || ex is BadRequestException)
        {
            loggerService.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            loggerService.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("available-slots")]
    public async Task<IActionResult> ListAvailableSlots(string? start, string? end)
    {
        try
        {
            IList<MeetingIntervalDTO> timeSlots = await meetingService.ListAvailableSlots(new(start, end));
            return Ok(
            new AvailableTimeSlots()
            {
                AvailableSlots = timeSlots
            });

        }
        catch (Exception ex) when (ex is ConflictTimeException || ex is BadRequestException)
        {
            loggerService.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            loggerService.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}

