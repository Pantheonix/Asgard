namespace Api.Features.Users.Delete;

public class DeleteUserEndpoint : Endpoint<DeleteUserRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DeleteUserEndpoint> _logger;

    public DeleteUserEndpoint(
        UserManager<ApplicationUser> userManager,
        ILogger<DeleteUserEndpoint> logger
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Delete("{id}");
        Roles(ApplicationRole.Admin.ToString());
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(DeleteUserRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Deleting user with id {Id}", req.Id.ToString());

        var userToDelete = await _userManager.FindByIdAsync(req.Id.ToString());

        if (userToDelete is null)
        {
            _logger.LogWarning("User with id {Id} not found", req.Id.ToString());
            await SendNotFoundAsync(ct);
            return;
        }

        var result = await _userManager.DeleteAsync(userToDelete);

        if (!result.Succeeded)
        {
            var errors = result
                .Errors.Select(e => e.Description)
                .Aggregate("Identity Errors: ", (a, b) => $"{a}, {b}");

            _logger.LogWarning(
                "User with id {Id} could not be deleted: {Errors}",
                req.Id.ToString(),
                errors
            );
            AddError(errors);
        }

        ThrowIfAnyErrors();

        await SendNoContentAsync(ct);
    }
}
