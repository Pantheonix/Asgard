namespace Api.Features.Users.GetAll;

public class ApplicationUserToUserDtoProfile : Profile
{
    public ApplicationUserToUserDtoProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName));
    }
}
