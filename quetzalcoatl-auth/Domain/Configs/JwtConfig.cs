namespace Domain.Configs;

public class JwtConfig
{
    public string SecretKey { get; init; } = string.Empty;
    public int JwtLifetime { get; init; } = 1;
}
