namespace Tests.Integration.Api.Features.Users;

public class GetEndpointTests : IClassFixture<ApiWebFactory>
{
    #region SetUp

    private readonly HttpClient _client;

    private readonly Faker<RegisterUserRequest> _registerUserRequestFaker =
        new Faker<RegisterUserRequest>()
            .RuleFor(rule => rule.Username, faker => faker.Internet.UserName().ClampLength(3))
            .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
            .RuleFor(rule => rule.Password, faker => faker.Internet.Password());

    public GetEndpointTests(ApiWebFactory apiWebFactory)
    {
        _client = apiWebFactory.CreateClient();
    }

    #endregion

    [Fact]
    public async Task GivenAnonymousUser_WhenGettingUser_ThenReturnsUnauthorized()
    {
        #region Arrange

        const string validPassword = "P@ssw0rd!";
        var registerUserRequest = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        await _client.POSTAsync<RegisterUserEndpoint, RegisterUserRequest, RegisterUserResponse>(
            registerUserRequest
        );

        var request = new GetUserRequest { Username = registerUserRequest.Username };

        #endregion

        #region Act

        var response = await _client.GETAsync<GetUserEndpoint, GetUserRequest>(request);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndExistingUserIdentity_WhenGettingUser_ThenReturnsOk()
    {
        #region Arrange

        const string validPassword = "P@ssw0rd!";
        var registerUserRequest = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        await _client.POSTAsync<RegisterUserEndpoint, RegisterUserRequest, RegisterUserResponse>(
            registerUserRequest
        );

        var loginUserRequest = new LoginUserRequest
        {
            Email = registerUserRequest.Email,
            Password = validPassword
        };

        var (_, loginResult) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            LoginUserResponse
        >(loginUserRequest);

        var token = loginResult!.Token;

        var request = new GetUserRequest { Username = registerUserRequest.Username };

        var expectedResponse = new GetUserResponse
        {
            Username = registerUserRequest.Username,
            Email = registerUserRequest.Email
        };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var (response, result) = await _client.GETAsync<
            GetUserEndpoint,
            GetUserRequest,
            GetUserResponse
        >(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResponse);

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndNonExistingUserIdentity_WhenGettingUser_ThenReturnsNotFound()
    {
        #region Arrange

        const string validPassword = "P@ssw0rd!";
        var registerUserRequest = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        await _client.POSTAsync<RegisterUserEndpoint, RegisterUserRequest, RegisterUserResponse>(
            registerUserRequest
        );

        var loginUserRequest = new LoginUserRequest
        {
            Email = registerUserRequest.Email,
            Password = validPassword
        };

        var (_, loginResult) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            LoginUserResponse
        >(loginUserRequest);

        var token = loginResult!.Token;

        var request = new GetUserRequest { Username = "non-existing-user" };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.GETAsync<
            GetUserEndpoint,
            GetUserRequest
        >(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        #endregion
    }
}
