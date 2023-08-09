using AutoMapper;

namespace EnkiProblems.Problems;

public class ProblemToProblemEvalMetadataDtoProfile : Profile
{
    public ProblemToProblemEvalMetadataDtoProfile()
    {
        CreateMap<Problem, ProblemEvalMetadataDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Limit.Time))
            .ForMember(dest => dest.TotalMemory, opt => opt.MapFrom(src => src.Limit.TotalMemory))
            .ForMember(dest => dest.StackMemory, opt => opt.MapFrom(src => src.Limit.StackMemory))
            .ForMember(dest => dest.IoType, opt => opt.MapFrom(src => src.IoType))
            .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests));
    }
}
