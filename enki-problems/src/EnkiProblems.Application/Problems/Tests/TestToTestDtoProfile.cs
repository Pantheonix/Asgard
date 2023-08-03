using AutoMapper;

namespace EnkiProblems.Problems.Tests;

public class TestToTestDtoProfile : Profile
{
    public TestToTestDtoProfile()
    {
        CreateMap<Test, TestDto>()
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));
    }
}
