namespace Api.Features.Auth.Register;

public class RegisterUserEndpoint : Endpoint<RegisterUserRequest, RegisterUserResponse>
{
    private readonly AutoMapper.IMapper _mapper;

    public RegisterUserEndpoint(AutoMapper.IMapper mapper)
    {
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("/api/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterUserRequest req, CancellationToken ct)
    {
        // create user using command
        var userRegistered = await _mapper.Map<CreateUserCommand>(req).ExecuteAsync(ct: ct);

        // generate token using command
        var token = await _mapper.Map<GenerateJwtTokenCommand>(userRegistered).ExecuteAsync(ct: ct);

        await SendOkAsync(
            new RegisterUserResponse
            {
                Email = userRegistered.Email!,
                Username = userRegistered.UserName!,
                Token = token
            },
            ct
        );
    }
}
