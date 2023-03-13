namespace Api.Features.Auth.Register;

public class RegisterUserRequestToCreateUserCommandProfile : Profile
{
    public RegisterUserRequestToCreateUserCommandProfile()
    {
        CreateMap<RegisterUserRequest, CreateUserCommand>();
    }
}
