namespace Api.Features.Users.GetAll;

public class ApplicationUserToUserDtoProfile : Profile
{
    public ApplicationUserToUserDtoProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(
                dest => dest.Fullname,
                opt =>
                {
                    opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.Fullname));
                    opt.MapFrom(src => src.Fullname);
                }
            )
            .ForMember(
                dest => dest.Bio,
                opt =>
                {
                    opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.Bio));
                    opt.MapFrom(src => src.Bio);
                }
            );
    }
}
