using MeetingScheduler.Core.Models.Constants;
using Xunit;

namespace MeetingScheduler.Test.Core.Models;

public class AppSettingsTest
{
    [Fact]
    public void Should_Get_And_Set_Properties()
    {
        AppSettings.MeetingHourLimitExceptionMessage = "";
        AppSettings.InvalidStartTimeExceptionMessage = "";
        AppSettings.BadTimeStampFormatExceptionMessage = "";
        _ = AppSettings.BadTimeStampFormatExceptionMessage;
        AppSettings.OutOfRangeExceptionMessage = "";
        AppSettings.ConflictTimeExceptionMessage = "";
        _ = AppSettings.ConflictTimeExceptionMessage;
        AppSettings.BadDateStringFormatExceptionMessage = "";
        _ = AppSettings.BadDateStringFormatExceptionMessage;
    }
}
