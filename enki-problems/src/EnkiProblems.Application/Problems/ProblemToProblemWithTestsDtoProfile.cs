using AutoMapper;

namespace EnkiProblems.Problems;

public class ProblemToProblemWithTestsDtoProfile : Profile
{
    public ProblemToProblemWithTestsDtoProfile()
    {
        CreateMap<Problem, ProblemWithTestsDto>()
            .ForMember(dest => dest.SourceName, opt => opt.MapFrom(src => src.Origin.SourceName))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Origin.AuthorName))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Limit.Time))
            .ForMember(dest => dest.TotalMemory, opt => opt.MapFrom(src => src.Limit.TotalMemory))
            .ForMember(dest => dest.StackMemory, opt => opt.MapFrom(src => src.Limit.StackMemory))
            .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests));
    }
}
