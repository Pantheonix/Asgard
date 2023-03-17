namespace Api.Features.Users.Get;

public class GetUserEndpoint : Endpoint<GetUserRequest, GetUserResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUserEndpoint(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("{id}");
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        var user = _userManager.Users.FirstOrDefault(user => req.Id == user.Id);

        if (user == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync(response: _mapper.Map<GetUserResponse>(user), ct);
    }
}
