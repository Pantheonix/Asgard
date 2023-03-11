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
        var createUserCommand = _mapper.Map<CreateUserCommand>(req);
        var userRegistered = await createUserCommand.ExecuteAsync(ct: ct);

        var generateJwtTokenCommand = _mapper.Map<GenerateJwtTokenCommand>(userRegistered);
        var token = await generateJwtTokenCommand.ExecuteAsync(ct: ct);

        await SendCreatedAtAsync(
            endpointName: "/api/users/{id}",
            routeValues: new { id = userRegistered.Id },
            responseBody: new RegisterUserResponse
            {
                Email = userRegistered.Email!,
                Username = userRegistered.UserName!,
                Token = token
            },
            cancellation: ct
        );
    }
}
