namespace Api.Features.Auth.RefreshToken;

public class UserTokenServiceEndpoint : RefreshTokenService<TokenRequest, UserTokenResponse>
{
    private readonly IRefreshTokenRepository _tokenRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<UserTokenServiceEndpoint> _logger;

    public UserTokenServiceEndpoint(
        JwtConfig jwtConfig,
        UserManager<ApplicationUser> userManager,
        IRefreshTokenRepository tokenRepository,
        IMapper mapper,
        ILogger<UserTokenServiceEndpoint> logger
    )
    {
        _tokenRepository =
            tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        Setup(opt =>
        {
            opt.TokenSigningKey = jwtConfig.SecretKey;
            opt.AccessTokenValidity = TimeSpan.FromMinutes(jwtConfig.JwtAccessTokenLifetime);
            opt.RefreshTokenValidity = TimeSpan.FromDays(jwtConfig.JwtRefreshTokenLifetime);
            opt.Endpoint(
                "/auth/refresh-token",
                ep =>
                {
                    ep.Summary(
                        s =>
                        {
                            s.Summary = "Refresh access token";
                            s.Description = "This endpoint is used to refresh the access token";
                        });
                }
            );
        });
    }

    public override Task PersistTokenAsync(UserTokenResponse response)
    {
        _logger.LogInformation("Persisting refresh token for user {UserId}", response.UserId);
        
        var token = _mapper.Map<Domain.Entities.RefreshToken>(response);
        return _tokenRepository.StoreRefreshTokenAsync(token, new CancellationToken());
    }

    public override async Task RefreshRequestValidationAsync(TokenRequest req)
    {
        _logger.LogInformation("Validating refresh token for user {UserId}", req.UserId);
        
        if (!await _tokenRepository.IsTokenValidAsync(req.UserId, req.RefreshToken, new CancellationToken()))
        {
            _logger.LogError("Invalid refresh token for user {UserId}", req.UserId);
            AddError("Invalid refresh token");
        }
    }

    public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
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
