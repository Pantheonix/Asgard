namespace Api.Features.Users.Roles.Add;

public class AddRoleEndpoint : Endpoint<AddRoleRequest, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<AddRoleEndpoint> _logger;

    public AddRoleEndpoint(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<AddRoleEndpoint> logger
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Post("{id}/role");
        Roles(ApplicationRole.Admin.ToString());
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(AddRoleRequest req, CancellationToken ct)
    {
        _logger.LogInformation(
            "Adding role {Role} to user with id {Id}",
            req.Role.ToString(),
            req.Id.ToString()
        );

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

        if (await _userManager.IsInRoleAsync(user, role.ToString()))
        {
            _logger.LogWarning(
                "User with id {Id} already has role {Role}",
                req.Id.ToString(),
                role.ToString()
            );
            var errors = $"User with id {req.Id.ToString()} already has role {role}";
            AddError(errors);
        }
        ThrowIfAnyErrors();

        var result = await _userManager.AddToRoleAsync(user, role.ToString());

        if (!result.Succeeded)
        {
            var errors = result
                .Errors.Select(e => e.Description)
                .Aggregate("Identity Errors: ", (a, b) => $"{a}, {b}");

            _logger.LogWarning(
                "Failed to add role {Role} to user with id {Id}: {errors}",
                role.ToString(),
                req.Id.ToString(),
                errors
            );
            AddError(errors);
        }
        ThrowIfAnyErrors();

        var updatedUser = await _userManager.FindByIdAsync(req.Id.ToString());
        var response = _mapper.Map<UserDto>(updatedUser);
        response.Roles = await _userManager.GetRolesAsync(updatedUser!);

        await SendOkAsync(response, ct);
    }
}
