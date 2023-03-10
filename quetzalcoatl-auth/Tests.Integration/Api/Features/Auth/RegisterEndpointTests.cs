using Api.Features.Auth.Register;

namespace Tests.Integration.Api.Features.Auth;

public class RegisterEndpointTests : IClassFixture<ApiWebFactory>
{
    private readonly ApiWebFactory _apiWebFactory;
    private readonly HttpClient _client;

    private readonly Faker<RegisterUserRequest> _registerUserRequestFaker =
        new Faker<RegisterUserRequest>()
            .RuleFor(rule => rule.Username, faker => faker.Internet.UserName().ClampLength(3))
            .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
            .RuleFor(
                rule => rule.Password,
                faker =>
                    faker.Internet.Password(
                        length: 10,
                        regexPattern: @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,20}$"
                    )
            );

    public RegisterEndpointTests(ApiWebFactory apiWebFactory)
    {
        _apiWebFactory = apiWebFactory;
        _client = _apiWebFactory.CreateClient();
    }

    [Fact]
    public async Task GivenValidUser_WhenRegistering_ThenReturnsOk()
    {
        // Arrange
        var request = _registerUserRequestFaker.Generate();

        // Act
        var (response, result) = await _client.POSTAsync<
            RegisterUserEndpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Should().NotBeNull();
        result!.Username.Should().Be(request.Username);
        result.Email.Should().Be(request.Email);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }
}
