namespace Api.Features.Users.Update;

public class UpdateUserRequest
{
    [FromRoute]
    public Guid Id { get; set; } = Guid.Empty;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}

public class UpdateUserResponse
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public Guid? ProfilePictureId { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
