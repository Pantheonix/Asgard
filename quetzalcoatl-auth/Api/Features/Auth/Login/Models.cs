namespace Api.Features.Auth.Login;

public class LoginUserRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class LoginUserResponse
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public string ProfilePictureUrl { get; set; } = default!;
    public string Token { get; set; } = default!;
}
