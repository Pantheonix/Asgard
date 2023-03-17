namespace Api.Features.Users.Update;

public class UpdateUserRequestToApplicationUserProfile : Profile
{
    public UpdateUserRequestToApplicationUserProfile()
    {
        CreateMap<UpdateUserRequest, ApplicationUser>()
            .ForMember(
                dest => dest.UserName,
                opt =>
                {
                    opt.PreCondition(src => !src.Username.IsNullOrEmpty());
                    opt.MapFrom(src => src.Username);
                }
            )
            .ForMember(
                dest => dest.Email,
                opt =>
                {
                    opt.PreCondition(src => !src.Email.IsNullOrEmpty());
                    opt.MapFrom(src => src.Email);
                }
            );
    }
}

public class ApplicationUserToUpdateUserResponseProfile : Profile
{
    public ApplicationUserToUpdateUserResponseProfile()
    {
        CreateMap<ApplicationUser, UpdateUserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}
