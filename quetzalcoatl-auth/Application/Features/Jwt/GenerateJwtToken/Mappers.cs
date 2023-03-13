namespace Application.Features.Jwt.GenerateJwtToken;

public class ApplicationUserToGenerateJwtTokenCommandProfile : Profile
{
    public ApplicationUserToGenerateJwtTokenCommandProfile()
    {
        CreateMap<ApplicationUser, GenerateJwtTokenCommand>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));
    }
}
