namespace Application.Features.Jwt;

public class GenerateJwtTokenCommand : ICommand<string>
{
    public ApplicationUser User { get; set; } = default!;
}

public class GenerateJwtTokenCommandHandler : CommandHandler<GenerateJwtTokenCommand, string>
{
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<ApplicationUser> _userManager;

    public GenerateJwtTokenCommandHandler(
        JwtConfig jwtConfig,
        UserManager<ApplicationUser> userManager
    )
    {
        _jwtConfig = jwtConfig;
        _userManager = userManager;
    }

    public override Task<string> ExecuteAsync(
        GenerateJwtTokenCommand command,
        CancellationToken ct = default
    )
    {
        var jwtToken = JWTBearer.CreateToken(
            signingKey: _jwtConfig.SecretKey,
            expireAt: DateTime.UtcNow.AddDays(_jwtConfig.JwtLifetime),
            priviledges: u =>
            {
                u.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, command.User.Id));
                u.Claims.Add(new Claim(JwtRegisteredClaimNames.Email, command.User.Email!));
                u.Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                u["userId"] = command.User.Id;
            }
        );

        return Task.FromResult(jwtToken);
    }
}