namespace Api.Features.Auth.RefreshToken;

public class UserTokenServiceEndpoint : RefreshTokenService<UserTokenRequest, UserTokenResponse>
{
    private readonly IRefreshTokenRepository _tokenRepository;
    private readonly TokenValidationParameters _tokenValidationParameters;
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
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        Setup(opt =>
        {
            opt.TokenSigningKey = jwtConfig.Value.SecretKey;
            opt.AccessTokenValidity = jwtConfig.Value.JwtAccessTokenLifetime;
            opt.RefreshTokenValidity = jwtConfig.Value.JwtRefreshTokenLifetime;
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

        var lastNotExpiredRefreshTokenUsedByUser = await _tokenRepository.GetRefreshTokenAsync(
            rt =>
                rt.UserId == Guid.Parse(response.UserId)
                && rt.ExpiryDate > DateTime.UtcNow
                && !rt.IsUsed
                && !rt.IsInvalidated,
            rtOrd => rtOrd.OrderByDescending(rt => rt.ExpiryDate)
        );

        if (lastNotExpiredRefreshTokenUsedByUser is not null)
        {
            token.CreationDate = lastNotExpiredRefreshTokenUsedByUser.CreationDate;
            token.ExpiryDate = lastNotExpiredRefreshTokenUsedByUser.ExpiryDate;
            token.IsUsed = false;
        }

        await _tokenRepository.CreateRefreshTokenAsync(token, new CancellationToken());
    }

    public override async Task RefreshRequestValidationAsync(UserTokenRequest req)
    {
        _logger.LogInformation(
            "Validating the refresh/access token pair for user {UserId}",
            req.UserId
        );

        var validatedAccessToken = req.AccessToken.ExtractValidatedClaimsPrincipal(
            _tokenValidationParameters
        );

        // check if the access token is valid
        if (validatedAccessToken is null)
        {
            _logger.LogError("Access token is invalid");
            AddError("Access token is invalid");
        }
        ThrowIfAnyErrors();

        // check if the access token is expired
        var expiryDateUnix = long.Parse(
            validatedAccessToken!.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value
        );

        var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(
            expiryDateUnix
        );

        if (expiryDateTimeUtc > DateTime.UtcNow)
        {
            _logger.LogError("Access token has not expired yet");
            AddError("Access token has not expired yet");
        }
        ThrowIfAnyErrors();

        // check if the refresh token exists in the database
        var jti = validatedAccessToken.Claims
            .Single(x => x.Type == JwtRegisteredClaimNames.Jti)
            .Value;

        var storedRefreshToken = await _tokenRepository.GetRefreshTokenAsync(
            rt =>
                rt.Token == Guid.Parse(req.RefreshToken)
                && rt.Jti == Guid.Parse(jti)
                && rt.UserId == Guid.Parse(req.UserId)
        );

        if (storedRefreshToken is null)
        {
            _logger.LogError("The refresh/access token pair does not exist");
            AddError("This refresh/access token pair does not exist");
        }
        ThrowIfAnyErrors();

        // check if the refresh token is expired
        if (DateTime.UtcNow > storedRefreshToken!.ExpiryDate)
        {
            _logger.LogError("This refresh token has expired");
            AddError("This refresh token has expired");
        }
        ThrowIfAnyErrors();

        // check if the refresh token has been invalidated
        if (storedRefreshToken.IsInvalidated)
        {
            _logger.LogError("This refresh token has been invalidated");
            AddError("This refresh token has been invalidated");
        }
        ThrowIfAnyErrors();

        // check if the refresh token has been used
        if (storedRefreshToken.IsUsed)
        {
            _logger.LogError("This refresh token has been used for the provided access token");
            AddError("This refresh token has been used for the provided access token");
        }
        ThrowIfAnyErrors();

        // mark refresh token as used
        storedRefreshToken.IsUsed = true;
        await _tokenRepository.UpdateRefreshTokenAsync(storedRefreshToken, new CancellationToken());
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
