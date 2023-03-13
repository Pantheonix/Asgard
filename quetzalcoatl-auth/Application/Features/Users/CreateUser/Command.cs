namespace Application.Features.Users.CreateUser;

public class CreateUserCommand : ICommand<ApplicationUser>
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
