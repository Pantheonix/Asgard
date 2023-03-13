namespace Tests.Integration.Api.Features.Auth;

public class LoginEndpointTests : IClassFixture<ApiWebFactory>
{
    private readonly HttpClient _client;

    private readonly Faker<RegisterUserRequest> _registerUserRequestFaker =
        new Faker<RegisterUserRequest>()
            .RuleFor(rule => rule.Username, faker => faker.Internet.UserName().ClampLength(3))
            .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
            .RuleFor(rule => rule.Password, faker => faker.Internet.Password());

    private readonly Faker<LoginUserRequest> _loginUserRequestFaker = new Faker<LoginUserRequest>()
        .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
        .RuleFor(rule => rule.Password, faker => faker.Internet.Password());

    public LoginEndpointTests(ApiWebFactory apiWebFactory)
    {
        _client = apiWebFactory.CreateClient();
    }

    [Fact]
    public async Task GivenValidUserCredentials_WhenLoggingIn_ThenReturnsOk()
    {
        // Arrange
        const string validPassword = "P@ssw0rd!";
        var registerRequest = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        await _client.POSTAsync<RegisterUserEndpoint, RegisterUserRequest, RegisterUserResponse>(
            registerRequest
        );

        var loginRequest = _loginUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Email, registerRequest.Email)
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        // Act
        var (response, result) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            LoginUserResponse
        >(loginRequest);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        result.Should().NotBeNull();
        result!.Username.Should().Be(registerRequest.Username);
        result.Email.Should().Be(registerRequest.Email);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GivenInvalidUserCredentials_WhenLoggingIn_ThenReturnsBadRequest()
    {
        // Arrange
        const string validPassword = "P@ssw0rd!";
        var registerRequest = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        await _client.POSTAsync<RegisterUserEndpoint, RegisterUserRequest, RegisterUserResponse>(
            registerRequest
        );

        const string invalidPassword = "P@ssw0rd!1";
        var loginRequest = _loginUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Email, registerRequest.Email)
            .RuleFor(rule => rule.Password, invalidPassword)
            .Generate();

        // Act
        var (response, result) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            ErrorResponse
        >(loginRequest);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        result.Should().NotBeNull();
        result!.Errors.Should().NotBeEmpty();
    }
}
