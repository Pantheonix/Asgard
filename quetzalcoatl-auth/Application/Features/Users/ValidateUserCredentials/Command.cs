namespace Application.Features.Users.ValidateUserCredentials;

public class ValidateUserCredentialsCommand : ICommand<ApplicationUser>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
