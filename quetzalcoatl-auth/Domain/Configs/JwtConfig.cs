namespace Domain.Configs;

public class JwtConfig
{
    public string SecretKey { get; init; } = string.Empty;
    public int JwtAccessTokenLifetime { get; init; } = 1;
    public int JwtRefreshTokenLifetime { get; init; } = 10;
}
