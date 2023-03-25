namespace Api.Features.Users.Update;

public class UpdateUserRequest
{
    public Guid Id { get; init; } = Guid.Empty;
    public string? Username { get; init; }
    public string? Email { get; init; }
    public string? Fullname { get; init; }
    public string? Bio { get; init; }
    public IFormFile? ProfilePicture { get; set; }
}

public class UpdateUserResponse
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public string ProfilePictureUrl { get; set; } = string.Empty;
}
