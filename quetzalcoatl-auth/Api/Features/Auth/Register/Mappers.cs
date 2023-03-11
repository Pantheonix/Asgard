using Application.Features.Jwt.GenerateJwtToken;
using Application.Features.Users.CreateUser;

namespace Api.Features.Auth.Register;

public class RegisterUserRequestToCreateUserCommandProfile : Profile
{
    public RegisterUserRequestToCreateUserCommandProfile()
    {
        CreateMap<RegisterUserRequest, CreateUserCommand>();
    }
}

public class ApplicationUserToGenerateJwtTokenCommandProfile : Profile
{
    public ApplicationUserToGenerateJwtTokenCommandProfile()
    {
        CreateMap<ApplicationUser, GenerateJwtTokenCommand>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));
    }
}
