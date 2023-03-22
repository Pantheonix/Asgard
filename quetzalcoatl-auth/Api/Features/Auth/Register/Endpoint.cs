﻿namespace Api.Features.Auth.Register;

public class RegisterUserEndpoint : Endpoint<RegisterUserRequest, RegisterUserResponse>
{
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterUserEndpoint> _logger;

    public RegisterUserEndpoint(IMapper mapper, ILogger<RegisterUserEndpoint> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Post("register");
        AllowFileUploads();
        Group<AuthenticationGroup>();
    }

    public override async Task HandleAsync(RegisterUserRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Registering user {Username}", req.Username);
        
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
                ProfilePictureUrl = user.GetProfilePictureUrl(
                    ProfilePictureConstants.BaseUrl,
                    ProfilePictureConstants.EndpointUrl,
                    ProfilePictureConstants.Extension
                ),
                Token = token
            },
            cancellation: ct
        );
    }
}
