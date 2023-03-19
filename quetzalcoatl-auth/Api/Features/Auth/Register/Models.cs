namespace Api.Features.Auth.Register;

public class RegisterUserRequest
{
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string? Fullname { get; init; }
    public string? Bio { get; init; }
}

public class RegisterUserResponse
{
    public Guid Id { get; set; }
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? Fullname { get; init; }
    public string? Bio { get; init; }
    public string Token { get; init; } = default!;
}
