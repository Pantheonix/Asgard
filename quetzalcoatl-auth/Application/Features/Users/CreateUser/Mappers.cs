namespace Application.Features.Users.CreateUser;

public class CreateUserCommandToApplicationUserProfile : Profile
{
    public CreateUserCommandToApplicationUserProfile()
    {
        CreateMap<CreateUserCommand, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}
