namespace Api.Features.Auth.Login;

public class LoginUserRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class LoginUserResponse
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
}
