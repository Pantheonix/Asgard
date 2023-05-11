using FastEndpoints.Security;

namespace Api.Features.Auth.Login;

public class LoginUserRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
