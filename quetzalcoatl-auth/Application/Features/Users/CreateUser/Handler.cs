namespace Application.Features.Users.CreateUser;

public class CreateUserCommandHandler : CommandHandler<CreateUserCommand, ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public override async Task<ApplicationUser> ExecuteAsync(
        CreateUserCommand command,
        CancellationToken ct = default
    )
    {
        var user = _mapper.Map<ApplicationUser>(command);
        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            AddError(
                result.Errors
                    .Select(e => e.Description)
                    .Aggregate("Identity Errors: ", (a, b) => $"{a}, {b}")
            );
        }

        ThrowIfAnyErrors();

        return user;
    }
}
