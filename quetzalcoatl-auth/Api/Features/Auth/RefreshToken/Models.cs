namespace Api.Features.Auth.RefreshToken;

public class UserTokenRequest : TokenRequest
{
    public string AccessToken { get; set; } = default!;
}

public class UserTokenResponse : TokenResponse
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public Guid? ProfilePictureId { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
