using MeetingScheduler.Core.Interfaces;
using MeetingScheduler.Core.Models;
using MeetingScheduler.Core.Models.Constants;
using MeetingScheduler.Core.Models.Entities;
using System.Globalization;

namespace MeetingScheduler.Core.Services;

public class IntervalUtil : IIntervalUtil
{
    const int minutesPerHour = 60;

    public IList<MeetingInterval> IntervalList { get; set; } = new List<MeetingInterval>();

    public IntervalUtil()
    {
        DateTime now = DateTime.Now;
        DateTime intervalStart = new(now.Year, now.Month, now.Day, AppSettings.StartOfDayHour, 0, 0);
        DateTime intervalEnd = new(now.Year, now.Month, now.Day, AppSettings.EndOfDayHour, 0, 0);
        DateTime counterTime = intervalStart;
        while (counterTime < intervalEnd)
        {
            DateTime thirtyMinutesTime = counterTime + new TimeSpan(0, (int)(AppSettings.MinMeetingHours * minutesPerHour), 0);
            IntervalList.Add(new() { Start = counterTime, End = thirtyMinutesTime });
            counterTime = thirtyMinutesTime;
        }
    }

    public static DateTime Parse(string dateString)
    {
        CultureInfo enUS = new("en-US");
        if (DateTime.TryParseExact(dateString, AppSettings.DateStringFormat, enUS, DateTimeStyles.RoundtripKind, out DateTime date))
        {
            return date;
        }
        return DateTime.MinValue;
    }

    public static bool IntervalIsAvailable(IList<Meeting> meetings, DateTime start, DateTime end)
    {
        for (int i = 0; i < meetings.Count; i++)
        {
            bool isBefore = end <= meetings[i].Start;
            bool isAfter = start >= meetings[i].End;
            if (!(isBefore || isAfter))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsTimeStampFormatValid(DateTime startDate, DateTime endDate)
    {
        bool startDateValid, endDateValid;
        startDateValid = endDateValid = false;
        for (int i = 0; i < IntervalList.Count; i++)
        {
            if (DateTime.Equals(startDate, IntervalList[i].Start))
            {
                startDateValid = true;
            }
            if (DateTime.Equals(endDate, IntervalList[i].End))
            {
                endDateValid = true;
            }
        }
        return startDateValid && endDateValid;
    }

    public bool IsWithinAllowableRange(DateTime startDate, DateTime endDate)
    {
        return !(startDate < IntervalList[0].Start || endDate > IntervalList.Last().End);
    }

    public static bool IsDateStringISO(string dateString)
    {
        if (Parse(dateString) != DateTime.MinValue)
        {
            return true;
        }
        return false;
    }

    public static string ParseToISOString(DateTime date)
    {
        DateTime newDate = DateTime.SpecifyKind(date, DateTimeKind.Local);
        return newDate.ToString(AppSettings.DateStringFormat);
    }
    public static bool IsStartInvalid(DateTime startDate, DateTime endDate)
    {
        return startDate < DateTime.Now || startDate >= endDate;
    }
}
