namespace Api.Features.Auth.Register;

public class RegisterUserRequest
{
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}

public class RegisterUserResponse
{
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Token { get; init; } = default!;
}
