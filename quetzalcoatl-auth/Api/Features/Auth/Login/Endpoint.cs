namespace Api.Features.Auth.Login;

public class LoginUserEndpoint : Endpoint<LoginUserRequest, UserTokenResponse>
{
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginUserEndpoint> _logger;

    public LoginUserEndpoint(
        IOptions<JwtConfig> jwtConfig,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<LoginUserEndpoint> logger
    )
    {
        _jwtConfig = jwtConfig.Value ?? throw new ArgumentNullException(nameof(jwtConfig));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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

        var userRoles = await _userManager.GetRolesAsync(user);
        var tokenResponse = await CreateTokenWith<UserTokenServiceEndpoint>(
            user.Id.ToString(),
            up =>
            {
                up.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
                up.Claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email!));
                up.Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                up.Roles.AddRange(userRoles);
            }
        );

        HttpContext.Response.Cookies.Append(
            CookieAuthenticationDefaults.CookiePrefix + "AccessToken",
            tokenResponse.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = false,
                Expires = DateTimeOffset.UtcNow.AddTicks(_jwtConfig.JwtAccessTokenLifetime.Ticks)
            }
        );

        HttpContext.Response.Cookies.Append(
            CookieAuthenticationDefaults.CookiePrefix + "RefreshToken",
            tokenResponse.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = false,
                Expires = DateTimeOffset.UtcNow.AddTicks(_jwtConfig.JwtRefreshTokenLifetime.Ticks)
            }
        );

        await SendOkAsync(
            response: new UserTokenResponse
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
                )
            },
            cancellation: ct
        );
    }
}
