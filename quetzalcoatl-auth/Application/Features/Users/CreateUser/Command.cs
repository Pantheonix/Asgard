namespace Application.Features.Users.CreateUser;

public class CreateUserCommand : ICommand<ApplicationUser>
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? Fullname { get; set; }
    public string? Bio { get; set; }
    public byte[]? ProfilePictureData { get; set; }
}
