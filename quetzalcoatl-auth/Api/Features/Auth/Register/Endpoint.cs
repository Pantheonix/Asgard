namespace Api.Features.Auth.Register;

public class RegisterUserEndpoint : Endpoint<RegisterUserRequest, UserTokenResponse>
{
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterUserEndpoint> _logger;

    public RegisterUserEndpoint(
        IOptions<JwtConfig> jwtConfig,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<RegisterUserEndpoint> logger
    )
    {
        _jwtConfig = jwtConfig.Value ?? throw new ArgumentNullException(nameof(jwtConfig));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Post("register");
        AllowFileUploads();
        Group<AuthenticationGroup>();
    }

    public override async Task HandleAsync(RegisterUserRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Registering user {Username}", req.Username);

        var createUserCommand = _mapper.Map<CreateUserCommand>(req);
        var user = await createUserCommand.ExecuteAsync(ct: ct);

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

        await SendCreatedAtAsync(
            endpointName: $"/api/users/{user.Id}",
            routeValues: new { id = user.Id },
            responseBody: new UserTokenResponse
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
            },
            cancellation: ct
        );
    }
}
