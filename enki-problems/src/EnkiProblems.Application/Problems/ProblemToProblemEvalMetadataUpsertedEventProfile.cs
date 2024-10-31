using AutoMapper;
using EnkiProblems.Problems.Events;

namespace EnkiProblems.Problems;

public class ProblemToProblemEvalMetadataUpsertedEventProfile : Profile
{
    public ProblemToProblemEvalMetadataUpsertedEventProfile()
    {
        CreateMap<Problem, ProblemEvalMetadataUpsertedEvent>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ProposerId, opt => opt.MapFrom(src => src.ProposerId))
            .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => src.IsPublished))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Limit.Time))
            .ForMember(dest => dest.TotalMemory, opt => opt.MapFrom(src => src.Limit.TotalMemory))
            .ForMember(dest => dest.StackMemory, opt => opt.MapFrom(src => src.Limit.StackMemory))
            .ForMember(dest => dest.IoType, opt => opt.MapFrom(src => src.IoType))
            .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests));
    }
}
