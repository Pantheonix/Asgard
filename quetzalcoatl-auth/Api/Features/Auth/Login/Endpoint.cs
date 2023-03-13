namespace Api.Features.Auth.Login;

public class LoginUserEndpoint : Endpoint<LoginUserRequest, LoginUserResponse>
{
    private readonly IMapper _mapper;

    public LoginUserEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("login");
        Group<AuthenticationGroup>();
    }

    public override async Task HandleAsync(LoginUserRequest req, CancellationToken ct)
    {
        var validateUserCredentialsCommand = _mapper.Map<ValidateUserCredentialsCommand>(req);
        var user = await validateUserCredentialsCommand.ExecuteAsync(ct: ct);

        var generateJwtTokenCommand = _mapper.Map<GenerateJwtTokenCommand>(user);
        var token = await generateJwtTokenCommand.ExecuteAsync(ct: ct);

        await SendOkAsync(
            response: new LoginUserResponse
            {
                Username = user.UserName!,
                Email = user.Email!,
                Token = token
            },
            cancellation: ct
        );
    }
}
