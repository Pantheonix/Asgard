namespace Api.Features.Users.Get;

public class GetUserEndpoint : Endpoint<GetUserRequest, GetUserResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserEndpoint> _logger;

    public GetUserEndpoint(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<GetUserEndpoint> logger
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Get("{id}");
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Getting user with id {Id}", req.Id.ToString());

        var user = await _userManager.FindByIdAsync(req.Id.ToString());

        if (user is null)
        {
            _logger.LogWarning("User with id {Id} not found", req.Id.ToString());
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(response: _mapper.Map<GetUserResponse>(user), ct);
    }
}
