using MeetingScheduler.Core.Models;

namespace MeetingScheduler.Core.Interfaces;

public interface IIntervalUtil
{
    IList<MeetingInterval> IntervalList { get; set; }

    bool IsTimeStampFormatValid(DateTime startDate, DateTime endDate);
    bool IsWithinAllowableRange(DateTime startDate, DateTime endDate);
}
