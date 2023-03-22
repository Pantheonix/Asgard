namespace Api.Features.Users.Get;

public class GetUserEndpoint : Endpoint<GetUserRequest, GetUserResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUserEndpoint(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override void Configure()
    {
        Get("{id}");
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(req.Id.ToString());

        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(response: _mapper.Map<GetUserResponse>(user), ct);
    }
}
