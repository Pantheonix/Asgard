using Microsoft.EntityFrameworkCore;

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

        var users = _userManager.Users.AsAsyncEnumerable().SelectAwait(async user =>
        {
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await _userManager.GetRolesAsync(user);
            return userDto;
        }).AsAsyncEnumerable();

        var totalCount = await users.CountAsync(cancellationToken: ct);

        if (!string.IsNullOrWhiteSpace(req.Username))
        {
            users = users.Where(user => user.Username.Contains(req.Username));
        }

        if (req.SortBy is not null)
        {
            users = users.SortUsers(req.SortBy.Value);
        }

        users = LinqExtensions.Paginate(users, req.Page ?? 1, req.PageSize ?? 10);

        await SendOkAsync(response: new GetAllUsersResponse { Users = users.ToEnumerable(), TotalCount = totalCount }, ct);
    }
}
