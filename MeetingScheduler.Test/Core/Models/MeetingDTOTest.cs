using MeetingScheduler.Core.Models.DTO;
using Xunit;

namespace MeetingScheduler.Test.Core.Models;

public class MeetingDTOTest
{
    [Fact]
    public void Should_Get_Properties()
    {
        MeetingDTO meeting = new();
        _ = meeting.Start;
        _ = meeting.End;
        _ = meeting.Owner;
    }
}
