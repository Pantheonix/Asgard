namespace Api.Features.Users.Update;

public class UpdateUserEndpoint : Endpoint<UpdateUserRequest, UpdateUserResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserEndpoint> _logger;

    public UpdateUserEndpoint(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<UpdateUserEndpoint> logger
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Put("{id}");
        AllowFileUploads();
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Updating user with id {Id}", req.Id.ToString());

        var subClaim = User
            .Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value)
            .FirstOrDefault();

        var isAllowed = subClaim is not null && subClaim == req.Id.ToString();

        var userToUpdate = await _userManager.FindByIdAsync(req.Id.ToString());

        if (userToUpdate is null || !isAllowed)
        {
            _logger.LogWarning(
                "User with id {Id} could not be updated due to lack of permissions",
                req.Id.ToString()
            );
            await SendForbiddenAsync(ct);
            return;
        }

        var result = await _userManager.UpdateAsync(_mapper.Map(req, userToUpdate));

        if (!result.Succeeded)
        {
            var errors = result
                .Errors.Select(e => e.Description)
                .Aggregate("Identity Errors: ", (a, b) => $"{a}, {b}");

            _logger.LogWarning(
                "Failed to update user with id {Id}: {Errors}",
                req.Id.ToString(),
                errors
            );
            AddError(errors);
        }

        ThrowIfAnyErrors();

        var updatedUser = await _userManager.FindByIdAsync(req.Id.ToString());
        var userRoles = await _userManager.GetRolesAsync(updatedUser!);
        var response = _mapper.Map<UpdateUserResponse>(updatedUser);
        response.Roles = userRoles;

        await SendOkAsync(response, ct);
    }
}
