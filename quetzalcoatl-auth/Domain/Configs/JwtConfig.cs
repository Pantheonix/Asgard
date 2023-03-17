namespace Domain.Configs;

public class JwtConfig
{
    public string SecretKey { get; set; } = string.Empty;
    public int JwtLifetime { get; set; }
}