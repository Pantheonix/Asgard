namespace Application.Features.Users.ValidateUserCredentials;

public class ValidateUserCredentialsCommandHandler
    : CommandHandler<ValidateUserCredentialsCommand, ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ValidateUserCredentialsCommandHandler> _logger;

    public ValidateUserCredentialsCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<ValidateUserCredentialsCommandHandler> logger
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<ApplicationUser> ExecuteAsync(
        ValidateUserCredentialsCommand command,
        CancellationToken ct = default
    )
    {
        // check if user exists
        _logger.LogInformation("Validate user credentials for {Email}", command.Email);
        var user = await _userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            _logger.LogWarning("User with email {Email} not found", command.Email);
            ThrowError("Invalid credentials");
        }
        ThrowIfAnyErrors();

        // check if password is correct
        _logger.LogInformation("Check password for user {Email}", command.Email);
        var result = await _userManager.CheckPasswordAsync(user, command.Password);

        if (!result)
        {
            _logger.LogWarning("Invalid password for user {Email}", command.Email);
            ThrowError("Invalid credentials");
        }
        ThrowIfAnyErrors();

        return user;
    }
}
