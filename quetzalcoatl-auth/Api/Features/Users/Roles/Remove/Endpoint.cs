namespace Api.Features.Users.Roles.Remove;

public class RemoveRoleEndpoint : Endpoint<RemoveRoleRequest, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<RemoveRoleEndpoint> _logger;

    public RemoveRoleEndpoint(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<RemoveRoleEndpoint> logger
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Delete("{id}/role");
        Roles(ApplicationRole.Admin.ToString());
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(RemoveRoleRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Removing role {Role} from user with id {Id}", req.Role.ToString(), req.Id.ToString());

        if (!Enum.TryParse<ApplicationRole>(req.Role, out var role))
        {
            _logger.LogWarning("Role {Role} is not a valid role", req.Role);
            var errors = $"Role {req.Role.ToString()} is not a valid role";
            AddError(errors);
        }
        ThrowIfAnyErrors();

        var user = await _userManager.FindByIdAsync(req.Id.ToString());

        if (user is null)
        {
            _logger.LogWarning("User with id {Id} not found", req.Id.ToString());
            await SendNotFoundAsync(ct);
            return;
        }

        if (!await _userManager.IsInRoleAsync(user, role.ToString()))
        {
            _logger.LogWarning("User with id {Id} does not have role {Role}", req.Id.ToString(), role.ToString());
            var errors = $"User with id {req.Id.ToString()} does not have role {role}";
            AddError(errors);
        }
        ThrowIfAnyErrors();

        var result = await _userManager.RemoveFromRoleAsync(user, role.ToString());

        if (!result.Succeeded)
        {
            var errors = result
                .Errors
                .Select(e => e.Description)
                .Aggregate("Identity Errors: ", (a, b) => $"{a}, {b}");

            _logger.LogWarning("Failed to remove role {Role} from user with id {Id}: {errors}", role.ToString(), req.Id.ToString(), errors);
            AddError(errors);
        }
        ThrowIfAnyErrors();

        var updatedUser = await _userManager.FindByIdAsync(req.Id.ToString());
        var response = _mapper.Map<UserDto>(updatedUser);

        await SendOkAsync(response, ct);
    }
}