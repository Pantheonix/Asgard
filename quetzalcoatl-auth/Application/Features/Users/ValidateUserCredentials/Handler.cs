namespace Application.Features.Users.ValidateUserCredentials;

public class ValidateUserCredentialsCommandHandler
    : CommandHandler<ValidateUserCredentialsCommand, ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ValidateUserCredentialsCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override async Task<ApplicationUser> ExecuteAsync(
        ValidateUserCredentialsCommand command,
        CancellationToken ct = default
    )
    {
        // check if user exists
        var user = await _userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            ThrowError("Invalid credentials");
        }

        // check if password is correct
        var result = await _userManager.CheckPasswordAsync(user, command.Password);

        if (!result)
        {
            ThrowError("Invalid credentials");
        }

        return user;
    }
}
