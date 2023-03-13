namespace Api.Features.Auth.Login;

public class LoginUserRequestToValidateUserCredentialsCommandProfile : Profile
{
    public LoginUserRequestToValidateUserCredentialsCommandProfile()
    {
        CreateMap<LoginUserRequest, ValidateUserCredentialsCommand>();
    }
}
