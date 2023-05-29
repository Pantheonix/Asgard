namespace Domain.Configs;

public class JwtConfig
{
    public string SecretKey { get; init; } = string.Empty;
    public TimeSpan JwtAccessTokenLifetime { get; init; } = TimeSpan.FromMinutes(1);
    public TimeSpan JwtRefreshTokenLifetime { get; init; } = TimeSpan.FromDays(10);
}
