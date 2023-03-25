namespace Api.Features.Auth.Login;

public class LoginUserRequest
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}

public class LoginUserResponse
{
    public Guid Id { get; init; } = Guid.Empty;
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? Fullname { get; init; }
    public string? Bio { get; init; }
    public string ProfilePictureUrl { get; init; } = default!;
    public string Token { get; init; } = default!;
}
