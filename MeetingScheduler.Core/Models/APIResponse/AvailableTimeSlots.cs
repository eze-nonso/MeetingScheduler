using MeetingScheduler.Core.Models.DTO;
using System.Text.Json.Serialization;

namespace MeetingScheduler.Core.Models.APIResponse;

public class AvailableTimeSlots
{
    [JsonPropertyName("available-slots")]
    public IList<MeetingIntervalDTO> AvailableSlots { get; set; } = default!;
}
