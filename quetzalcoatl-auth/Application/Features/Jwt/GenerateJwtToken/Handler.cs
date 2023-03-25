namespace Application.Features.Jwt.GenerateJwtToken;

public class GenerateJwtTokenCommandHandler : CommandHandler<GenerateJwtTokenCommand, string>
{
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GenerateJwtTokenCommandHandler> _logger;

    public GenerateJwtTokenCommandHandler(
        JwtConfig jwtConfig,
        UserManager<ApplicationUser> userManager,
        ILogger<GenerateJwtTokenCommandHandler> logger
    )
    {
        _jwtConfig = jwtConfig ?? throw new ArgumentNullException(nameof(jwtConfig));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task<string> ExecuteAsync(
        GenerateJwtTokenCommand command,
        CancellationToken ct = default
    )
    {
        _logger.LogInformation("Generate JWT token for user {Email}", command.User.Email);
        
        var userRoles = _userManager.GetRolesAsync(command.User).Result;

        var jwtToken = JWTBearer.CreateToken(
            signingKey: _jwtConfig.SecretKey,
            expireAt: DateTime.UtcNow.AddDays(_jwtConfig.JwtLifetime),
            priviledges: u =>
            {
                u.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, command.User.Id.ToString()));
                u.Claims.Add(new Claim(JwtRegisteredClaimNames.Email, command.User.Email!));
                u.Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                u.Roles.AddRange(userRoles);
            }
        );

        return Task.FromResult(jwtToken);
    }
}
