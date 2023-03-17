namespace Api.Features.Users.Update;

public class UpdateUserEndpoint : Endpoint<UpdateUserRequest, UpdateUserResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UpdateUserEndpoint(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Put("{id}");
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
    {
        var userToUpdate = await _userManager.FindByIdAsync(req.Id.ToString());

        if (userToUpdate == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var result = await _userManager.UpdateAsync(_mapper.Map(req, userToUpdate));

        if (!result.Succeeded)
        {
            AddError(
                result.Errors
                    .Select(e => e.Description)
                    .Aggregate("Identity Errors: ", (a, b) => $"{a}, {b}")
            );
        }

        ThrowIfAnyErrors();

        var updatedUser = await _userManager.FindByIdAsync(req.Id.ToString());

        await SendOkAsync(response: _mapper.Map<UpdateUserResponse>(updatedUser), ct);
    }
}
