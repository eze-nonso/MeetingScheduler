using AutoMapper;
using MeetingScheduler.Core.Interfaces;
using MeetingScheduler.Core.Models;
using MeetingScheduler.Core.Models.Constants;
using MeetingScheduler.Core.Models.DTO;
using MeetingScheduler.Core.Models.Entities;

namespace MeetingScheduler.Core.Services;
public class MeetingService : IMeetingService
{
    public MeetingService(IRepo repo, IIntervalUtil intervalUtil, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
        _intervalUtil = intervalUtil;
    }
    private readonly IRepo _repo;
    private readonly IIntervalUtil _intervalUtil;
    private readonly IMapper _mapper;
    public async Task<IList<MeetingDTO>> ListMeetings(DateRangeParam dateParam)
    {
        IList<Meeting> meetings = await _repo.ListMeetings(dateParam.Start, dateParam.End);
        return _mapper.Map<IList<MeetingDTO>>(meetings);
    }

    private async Task<IList<Meeting>> ListAllMeetings()
    {
        return await _repo.ListMeetings();
    }

    public async Task<string> CreateMeeting(NewMeetingReq newMeetingReq)
    {
        if (!(IntervalUtil.IsDateStringISO(newMeetingReq.Start) && IntervalUtil.IsDateStringISO(newMeetingReq.End)))
        {
            throw new BadRequestException(AppSettings.BadDateStringFormatExceptionMessage);
        }
        DateTime startDate = IntervalUtil.Parse(newMeetingReq.Start);
        DateTime endDate = IntervalUtil.Parse(newMeetingReq.End);
        if (IntervalUtil.IsStartInvalid(startDate, endDate))
        {
            throw new BadRequestException(AppSettings.InvalidStartTimeExceptionMessage);
        }

        double hoursInterval = (endDate - startDate).TotalHours;

        if (hoursInterval > AppSettings.MaxMeetingHours || hoursInterval < AppSettings.MinMeetingHours)
        {
            throw new BadRequestException(AppSettings.MeetingHourLimitExceptionMessage);
        }

        if (!_intervalUtil.IsWithinAllowableRange(startDate, endDate))
        {
            throw new BadRequestException(AppSettings.OutOfRangeExceptionMessage);
        }

        if (!_intervalUtil.IsTimeStampFormatValid(startDate, endDate))
        {
            throw new BadRequestException(AppSettings.BadTimeStampFormatExceptionMessage);
        }


        var meetings = await ListAllMeetings();
        if (!IntervalUtil.IntervalIsAvailable(meetings, startDate, endDate))
        {
            throw new ConflictTimeException(AppSettings.ConflictTimeExceptionMessage);
        }

        Meeting newMeeting = new()
        {
            Start = startDate,
            End = endDate,
            Owner = newMeetingReq.Owner
        };

        await _repo.CreateMeeting(newMeeting);
        return newMeeting.MeetingId.ToString();
    }

    public async Task<IList<MeetingIntervalDTO>> ListAvailableSlots(DateRangeParam timeRange)
    {
        IList<Meeting> meetings = await ListAllMeetings();
        IList<MeetingInterval> intervals = _intervalUtil.IntervalList
            .Where(interval => interval.Start >= timeRange.Start && interval.End <= timeRange.End && !IntervalUtil.IsStartInvalid(interval.Start, interval.End))
            .Where(interval => IntervalUtil.IntervalIsAvailable(meetings, interval.Start, interval.End)).ToList();
        return _mapper.Map<IList<MeetingIntervalDTO>>(intervals);
    }
}
