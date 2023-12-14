namespace Api.Features.Users.Core;

public class UserDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public Guid? ProfilePictureId { get; set; }
}
