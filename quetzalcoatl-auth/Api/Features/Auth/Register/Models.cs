namespace Api.Features.Auth.Register;

public class RegisterUserRequest
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}

public class RegisterUserResponse
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string Token { get; set; } = default!;
}
