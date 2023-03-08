public class JwtConfig
{
    public string SecretKey { get; set; } = String.Empty;
    public int JwtLifetime { get; set; }
}
