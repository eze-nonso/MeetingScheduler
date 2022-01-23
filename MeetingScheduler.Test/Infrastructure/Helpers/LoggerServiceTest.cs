using MeetingScheduler.Core.Interfaces.Helpers;
using MeetingScheduler.Core.Models.Constants;
using MeetingScheduler.Infrastructure.Helpers;
using System;
using Xunit;

namespace MeetingScheduler.Test.Infrastructure;

public class LoggerServiceTest
{
    [Fact]
    public void LogInfo_Should_Not_Throw_On_Invalid_Path()
    {
        AppSettings.Logpath = "";
        ILoggerService logger = new LoggerService();
        logger.LogInfo("Hello World");
        logger.LogInfo("Hello Mars");
    }


    [Fact]
    public void LogError_Should_Not_Throw_On_Invalid_Path()
    {
        AppSettings.Logpath = "";
        ILoggerService logger = new LoggerService();
        logger.LogError(new Exception("Log error test 1"), "Test log-error");
    }
}
