using Namotion.Reflection;

namespace Api.Features.Users.Get;

public class ApplicationUserToGetUserResponseProfile : Profile
{
    public ApplicationUserToGetUserResponseProfile()
    {
        CreateMap<ApplicationUser, GetUserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}
