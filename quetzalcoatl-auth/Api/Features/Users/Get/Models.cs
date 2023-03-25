namespace Api.Features.Users.Get;

public class GetUserRequest
{
    public Guid Id { get; set; } = Guid.Empty;
}

public class GetUserResponse
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
}
