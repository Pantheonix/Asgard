namespace Api.Features.Users.GetAll;

public class GetAllUsersEndpoint : Endpoint<GetAllUsersRequest, GetAllUsersResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllUsersEndpoint> _logger;

    public GetAllUsersEndpoint(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<GetAllUsersEndpoint> logger
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Get("/");
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(GetAllUsersRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Getting all users");
        
        var users = _userManager.Users.Select(user => _mapper.Map<UserDto>(user));

        await SendOkAsync(response: new GetAllUsersResponse { Users = users }, ct);
    }
}
