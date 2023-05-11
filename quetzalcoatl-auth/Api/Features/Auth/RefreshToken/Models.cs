using FastEndpoints.Security;

namespace Api.Features.Auth.RefreshToken;

public class UserTokenResponse : TokenResponse
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
}
