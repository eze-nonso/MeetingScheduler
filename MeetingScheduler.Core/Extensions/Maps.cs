using AutoMapper;
using MeetingScheduler.Core.Models;
using MeetingScheduler.Core.Models.DTO;
using MeetingScheduler.Core.Models.Entities;
using MeetingScheduler.Core.Services;

namespace MeetingScheduler.Core.Extensions;

public class Maps : Profile
{
    public Maps()
    {
        CreateMap<Meeting, MeetingDTO>()
            .ForMember(
                dest => dest.Start,
                opts => opts.MapFrom(
                    src => IntervalUtil.ParseToISOString(src.Start)
                    )
                )
            .ForMember(
                dest => dest.End,
                opts => opts.MapFrom(
                    src => IntervalUtil.ParseToISOString(src.End)
                    )
                );

        CreateMap<MeetingInterval, MeetingIntervalDTO>()
            .ForMember(
                dest => dest.Start,
                opts => opts.MapFrom(
                    src => IntervalUtil.ParseToISOString(src.Start)
                    )
                )
            .ForMember(
                dest => dest.End,
                opts => opts.MapFrom(
                    src => IntervalUtil.ParseToISOString(src.End)
                    )
                );
    }
}
