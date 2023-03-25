namespace Api.Features.Auth.Register;

public class RegisterUserRequestToCreateUserCommandProfile : Profile
{
    public RegisterUserRequestToCreateUserCommandProfile()
    {
        CreateMap<RegisterUserRequest, CreateUserCommand>()
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
                dest => dest.ProfilePictureData,
                opt =>
                {
                    opt.PreCondition(src => src.ProfilePicture is not null);
                    opt.MapFrom(src => src.ProfilePicture!.GetBytes());
                }
            );
    }
}
