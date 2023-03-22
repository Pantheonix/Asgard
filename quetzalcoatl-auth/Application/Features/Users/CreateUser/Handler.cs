namespace Application.Features.Users.CreateUser;

public class CreateUserCommandHandler : CommandHandler<CreateUserCommand, ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<CreateUserCommandHandler> logger
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<ApplicationUser> ExecuteAsync(
        CreateUserCommand command,
        CancellationToken ct = default
    )
    {
        _logger.LogInformation("Creating user {Username}", command.Username);

        var user = _mapper.Map<ApplicationUser>(command);
        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => e.Description)
                .Aggregate("Identity Errors: ", (a, b) => $"{a}, {b}");

            _logger.LogError(
                "Failed to create user {Username}: {Errors}",
                command.Username,
                errors
            );
            AddError(errors);
        }

        ThrowIfAnyErrors();

        return user;
    }
}
