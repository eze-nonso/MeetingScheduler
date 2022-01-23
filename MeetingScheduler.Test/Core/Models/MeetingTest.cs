using MeetingScheduler.Core.Models.Entities;
using Xunit;

namespace MeetingScheduler.Test.Core.Models;

public class MeetingTest
{
    [Fact]
    public void Should_Get_Properties()
    {
        Meeting meeting = new();
        _ = meeting.CreatedAt;
    }
}
