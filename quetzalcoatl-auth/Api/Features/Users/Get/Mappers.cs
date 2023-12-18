namespace Api.Features.Users.Get;

public class ApplicationUserToGetUserResponseProfile : Profile
{
    public ApplicationUserToGetUserResponseProfile()
    {
        CreateMap<ApplicationUser, GetUserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
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
            )
            .ForMember(
                dest => dest.ProfilePictureId,
                opt =>
                   opt.MapFrom<Guid?>(
                        src => src.ProfilePicture != null ? src.ProfilePicture!.Id : null
                        )
            );
    }
}
