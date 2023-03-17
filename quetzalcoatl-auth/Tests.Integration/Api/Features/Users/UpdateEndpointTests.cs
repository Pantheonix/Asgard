namespace Tests.Integration.Api.Features.Users;

public class UpdateEndpointTests : IClassFixture<ApiWebFactory>
{
    private readonly HttpClient _client;

    private readonly Faker<RegisterUserRequest> _registerUserRequestFaker =
        new Faker<RegisterUserRequest>()
            .RuleFor(rule => rule.Username, faker => faker.Internet.UserName().ClampLength(3))
            .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
            .RuleFor(rule => rule.Password, faker => faker.Internet.Password());

    public UpdateEndpointTests(ApiWebFactory apiWebFactory)
    {
        _client = apiWebFactory.CreateClient();
    }

    [Fact]
    public async Task GivenAnonymousUser_WhenUpdatingUser_ThenReturnsUnauthorized()
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

        var request = new UpdateUserRequest
        {
            Username = $"{registerUserRequest.Username}-updated",
            Email = $"{registerUserRequest.Email}-updated"
        };

        #endregion

        #region Act

        var response = await _client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest>(request);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndInvalidRequest_WhenUpdatingUser_ThenReturnsBadRequest()
    {
        #region Arrange

        const string validPassword = "P@ssw0rd!";
        var registerUserRequest = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        var (_, registerUserResponse) = await _client.POSTAsync<
            RegisterUserEndpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(registerUserRequest);

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

        var request = new UpdateUserRequest
        {
            Id = registerUserResponse!.Id,
            Username = $"{registerUserRequest.Username}-updated",
            Email = $"invalid-email"
        };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var (response, result) = await _client.PUTAsync<
            UpdateUserEndpoint,
            UpdateUserRequest,
            ErrorResponse
        >(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result.Should().NotBeNull();
        result!.Errors.Keys.Should().Contain(nameof(request.Email));

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndRequestForNonExistingUser_WhenUpdatingUser_ThenReturnsNotFound()
    {
        #region Arrange

        var users = new List<RegisterUserRequest>();

        const string validPassword = "P@ssw0rd!";
        for (var i = 0; i < 3; i++)
        {
            var registerUserRequest = _registerUserRequestFaker
                .Clone()
                .RuleFor(rule => rule.Password, $"{validPassword}{i}")
                .Generate();

            users.Add(registerUserRequest);

            await _client.POSTAsync<
                RegisterUserEndpoint,
                RegisterUserRequest,
                RegisterUserResponse
            >(registerUserRequest);
        }

        const string userPassword = $"{validPassword}0";
        var loginUserRequest = new LoginUserRequest
        {
            Email = users.ElementAt(0).Email,
            Password = userPassword
        };

        var (_, loginResult) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            LoginUserResponse
        >(loginUserRequest);

        var token = loginResult!.Token;

        var user = users.ElementAt(0);
        var request = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            Username = $"{user.Username}-updated",
            Email = $"{user.Email}-updated"
        };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest>(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndValidRequest_WhenUpdatingUser_ThenReturnsOk()
    {
        #region Arrange

        const string validPassword = "P@ssw0rd!";
        var registerUserRequest = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        var (_, registerUserResponse) = await _client.POSTAsync<
            RegisterUserEndpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(registerUserRequest);

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

        var request = new UpdateUserRequest
        {
            Id = registerUserResponse!.Id,
            Username = $"{registerUserRequest.Username}-updated",
            Email = $"{registerUserRequest.Email}-updated"
        };

        var expectedResponse = new UpdateUserResponse
        {
            Id = registerUserResponse.Id,
            Username = request.Username,
            Email = request.Email
        };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var (response, result) = await _client.PUTAsync<
            UpdateUserEndpoint,
            UpdateUserRequest,
            UpdateUserResponse
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
}
