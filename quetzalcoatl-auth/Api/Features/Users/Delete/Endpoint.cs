namespace Api.Features.Users.Delete;

public class DeleteUserEndpoint : Endpoint<DeleteUserRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Delete("{id}");
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(DeleteUserRequest req, CancellationToken ct)
    {
        var userToDelete = await _userManager.FindByIdAsync(req.Id.ToString());

        if (userToDelete is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var result = await _userManager.DeleteAsync(userToDelete);

        if (!result.Succeeded)
        {
            AddError(
                result.Errors
                    .Select(e => e.Description)
                    .Aggregate("Identity Errors: ", (a, b) => $"{a}, {b}")
            );
        }

        ThrowIfAnyErrors();

        await SendNoContentAsync(ct);
    }
}
