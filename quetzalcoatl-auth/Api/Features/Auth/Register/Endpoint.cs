namespace Api.Features.Auth.Register;

public class RegisterUserEndpoint : Endpoint<RegisterUserRequest, RegisterUserResponse>
{
    private readonly IMapper _mapper;

    public RegisterUserEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("register");
        AllowFileUploads();
        Group<AuthenticationGroup>();
    }

    public override async Task HandleAsync(RegisterUserRequest req, CancellationToken ct)
    {
        var createUserCommand = _mapper.Map<CreateUserCommand>(req);
        var user = await createUserCommand.ExecuteAsync(ct: ct);

        var generateJwtTokenCommand = _mapper.Map<GenerateJwtTokenCommand>(user);
        var token = await generateJwtTokenCommand.ExecuteAsync(ct: ct);

        await SendCreatedAtAsync(
            endpointName: "/api/users/{id}",
            routeValues: new { id = user.Id },
            responseBody: new RegisterUserResponse
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                Fullname = user.Fullname,
                Bio = user.Bio,
                ProfilePictureUrl = $"/api/images/{user.ProfilePicture.Id.ToString()}.jpg",
                Token = token
            },
            cancellation: ct
        );
    }
}
