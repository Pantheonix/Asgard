namespace Tests.Integration.Api.Features.Auth;

public class RegisterEndpointTests : IClassFixture<ApiWebFactory>
{
    private readonly HttpClient _client;

    private readonly Faker<RegisterUserRequest> _registerUserRequestFaker =
        new Faker<RegisterUserRequest>()
            .RuleFor(rule => rule.Username, faker => faker.Internet.UserName().ClampLength(3))
            .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
            .RuleFor(rule => rule.Password, faker => faker.Internet.Password());

    public RegisterEndpointTests(ApiWebFactory apiWebFactory)
    {
        _client = apiWebFactory.CreateClient();
    }

    [Fact]
    public async Task GivenValidUser_WhenRegistering_ThenReturnsCreated()
    {
        // Arrange
        const string validPassword = "P@ssw0rd!";
        var request = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        // Act
        var (response, result) = await _client.POSTAsync<
            RegisterUserEndpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        result.Should().NotBeNull();
        result!.Username.Should().Be(request.Username);
        result.Email.Should().Be(request.Email);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GivenInvalidUser_WhenRegistering_ThenReturnsBadRequest()
    {
        // Arrange
        var request = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Username, faker => faker.Random.String2(2))
            .Generate();

        // Act
        var (response, result) = await _client.POSTAsync<
            RegisterUserEndpoint,
            RegisterUserRequest,
            ErrorResponse
        >(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result.Should().NotBeNull();
        result!.Errors.Keys.Should().Contain(nameof(request.Username));
    }
}
