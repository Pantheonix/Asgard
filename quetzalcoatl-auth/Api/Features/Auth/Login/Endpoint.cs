namespace Api.Features.Auth.Login;

public class LoginUserEndpoint : Endpoint<LoginUserRequest, LoginUserResponse>
{
    private readonly IMapper _mapper;
    private readonly ILogger<LoginUserEndpoint> _logger;

    public LoginUserEndpoint(IMapper mapper, ILogger<LoginUserEndpoint> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Post("login");
        Group<AuthenticationGroup>();
    }

    public override async Task HandleAsync(LoginUserRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Login user {Email}", req.Email);
        
        var validateUserCredentialsCommand = _mapper.Map<ValidateUserCredentialsCommand>(req);
        var user = await validateUserCredentialsCommand.ExecuteAsync(ct: ct);

        var generateJwtTokenCommand = _mapper.Map<GenerateJwtTokenCommand>(user);
        var token = await generateJwtTokenCommand.ExecuteAsync(ct: ct);

        await SendOkAsync(
            response: new LoginUserResponse
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                Fullname = user.Fullname,
                Bio = user.Bio,
                ProfilePictureUrl = user.GetProfilePictureUrl(
                    ProfilePictureConstants.BaseUrl,
                    ProfilePictureConstants.EndpointUrl,
                    ProfilePictureConstants.Extension
                ),
                Token = token
            },
            cancellation: ct
        );
    }
}
