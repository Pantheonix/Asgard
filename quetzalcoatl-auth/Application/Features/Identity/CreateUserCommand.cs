namespace Application.Features.Identity;

public class CreateUserCommand : ICommand<ApplicationUser>
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class CreateUserCommandHandler : CommandHandler<CreateUserCommand, ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override async Task<ApplicationUser> ExecuteAsync(
        CreateUserCommand command,
        CancellationToken ct = default
    )
    {
        var user = new ApplicationUser { Email = command.Email, UserName = command.UserName };

        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            AddError(
                result.Errors
                    .Select(e => e.Description)
                    .Aggregate("Identity Errors: ", (a, b) => $"{a}, {b}")
            );
        }

        ThrowIfAnyErrors();

        return user;
    }
}
