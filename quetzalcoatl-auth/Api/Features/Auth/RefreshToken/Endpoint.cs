namespace Api.Features.Auth.RefreshToken;

public class UserTokenServiceEndpoint : RefreshTokenService<UserTokenRequest, UserTokenResponse>
{
    private readonly IRefreshTokenRepository _tokenRepository;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<UserTokenServiceEndpoint> _logger;

    public UserTokenServiceEndpoint(
        IOptions<JwtConfig> jwtConfig,
        TokenValidationParameters tokenValidationParameters,
        UserManager<ApplicationUser> userManager,
        IRefreshTokenRepository tokenRepository,
        IMapper mapper,
        ILogger<UserTokenServiceEndpoint> logger
    )
    {
        _tokenRepository =
            tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
        _tokenValidationParameters =
            tokenValidationParameters
            ?? throw new ArgumentNullException(nameof(tokenValidationParameters));
        _jwtConfig = jwtConfig.Value ?? throw new ArgumentNullException(nameof(jwtConfig));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        Setup(opt =>
        {
            opt.TokenSigningKey = _jwtConfig.SecretKey;
            opt.AccessTokenValidity = _jwtConfig.JwtAccessTokenLifetime;
            opt.RefreshTokenValidity = _jwtConfig.JwtRefreshTokenLifetime;
            opt.Endpoint(
                "/auth/refresh-token",
                ep =>
                {
                    ep.Summary(s =>
                    {
                        s.Summary = "Refresh access token";
                        s.Description = "This endpoint is used to refresh the access token";
                    });
                }
            );
        });
    }

    public override async Task PersistTokenAsync(UserTokenResponse response)
    {
        _logger.LogInformation(
            "Persisting refresh/access token pair for user {UserId}",
            response.UserId
        );

        var token = _mapper.Map<Domain.Entities.RefreshToken>(response);
        await _tokenRepository.CreateRefreshTokenAsync(token, new CancellationToken());
    }

    public override async Task RefreshRequestValidationAsync(UserTokenRequest req)
    {
        _logger.LogInformation(
            "Validating the refresh token for user {UserId}",
            req.UserId
        );

        var storedRefreshToken = await _tokenRepository.GetRefreshTokenAsync(
            rt =>
                rt.Token == Guid.Parse(req.RefreshToken)
                && rt.UserId == Guid.Parse(req.UserId)
                && rt.ExpiryDate > DateTime.UtcNow
        );

        if (storedRefreshToken is null)
        {
            _logger.LogError("The refresh token has expired or does not exist in the database");
            AddError("The refresh token has expired or does not exist in the database");
        }
        ThrowIfAnyErrors();

        // check if the refresh token has been invalidated
        if (storedRefreshToken!.IsInvalidated)
        {
            _logger.LogError("The refresh token has been invalidated");
            AddError("The refresh token has been invalidated");
        }
        ThrowIfAnyErrors();
    }

    public override async Task SetRenewalPrivilegesAsync(
        UserTokenRequest request,
        UserPrivileges privileges
    )
    {
        _logger.LogInformation("Setting renewal privileges for user {UserId}", request.UserId);

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            _logger.LogError("User {UserId} not found", request.UserId);
            AddError("User not found");
        }
        ThrowIfAnyErrors();

        var userRoles = await _userManager.GetRolesAsync(user!);

        privileges.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, request.UserId));
        privileges.Claims.Add(new Claim(JwtRegisteredClaimNames.Email, user!.Email!));
        privileges.Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        privileges.Roles.AddRange(userRoles);
    }
}
