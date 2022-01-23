namespace MeetingScheduler.Core.Models.Constants
{
    public class AppSettings
    {
        public static string Logpath { get; set; } = string.Empty;
        public static short EndOfDayHour { get; set; }
        public static short StartOfDayHour { get; set; }
        public static short MaxMeetingHours { get; set; }
        public static double MinMeetingHours { get; set; }
        public static string MeetingHourLimitExceptionMessage { get; set; } = string.Empty;
        public static string InvalidStartTimeExceptionMessage { get; set; } = string.Empty;
        public static string BadTimeStampFormatExceptionMessage { get; set; } = string.Empty;
        public static string OutOfRangeExceptionMessage { get; set; } = string.Empty;
        public static string ConflictTimeExceptionMessage { get; set; } = string.Empty;
        public static string BadDateStringFormatExceptionMessage { get; set; } = string.Empty;
        public static string DateStringFormat { get; set; } = string.Empty;
    }
}
