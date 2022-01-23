using MeetingScheduler.Core.Models.Constants;
using MeetingScheduler.Core.Services;

namespace MeetingScheduler.Core.Models;

public class DateRangeParam
{
    public DateRangeParam(string? Start, string? End)
    {
        start = Start;
        end = End;
    }
    private readonly string? start;
    private readonly string? end;

    public DateTime Start
    {
        get
        {
            if (string.IsNullOrEmpty(start))
            {
                return DateTime.Now;
            }
            return IntervalUtil.Parse(start);
        }
    }
    public DateTime End
    {
        get
        {
            if (string.IsNullOrEmpty(end))
            {
                DateTime now = DateTime.Now;
                return new DateTime(now.Year, now.Month, now.Day, AppSettings.EndOfDayHour, 0, 0);
            }
            return IntervalUtil.Parse(end);
        }
    }
}
