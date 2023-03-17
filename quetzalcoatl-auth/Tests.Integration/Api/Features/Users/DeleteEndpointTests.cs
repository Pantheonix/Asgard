namespace Tests.Integration.Api.Features.Users;

public class DeleteEndpointTests : IClassFixture<ApiWebFactory>
{
    #region SetUp

    private readonly HttpClient _client;

    private readonly Faker<RegisterUserRequest> _registerUserRequestFaker =
        new Faker<RegisterUserRequest>()
            .RuleFor(rule => rule.Username, faker => faker.Internet.UserName().ClampLength(3))
            .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
            .RuleFor(rule => rule.Password, faker => faker.Internet.Password());

    public DeleteEndpointTests(ApiWebFactory apiWebFactory)
    {
        _client = apiWebFactory.CreateClient();
    }

    #endregion

    [Fact]
    public async Task GivenAnonymousUser_WhenDeletingUser_ThenReturnsUnauthorized()
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

        var request = new DeleteUserRequest { Id = registerUserResponse!.Id };

        #endregion

        #region Act

        var response = await _client.DELETEAsync<DeleteUserEndpoint, DeleteUserRequest>(request);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndIdOfNonExistingUser_WhenDeletingUser_ThenReturnsNotFound()
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

        var request = new DeleteUserRequest { Id = Guid.NewGuid() };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.DELETEAsync<DeleteUserEndpoint, DeleteUserRequest>(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUser_WhenDeletingUser_ThenReturnsNoContent()
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

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var (_, getAllUsersResponse) = await _client.GETAsync<
            GetAllUsersEndpoint,
            GetAllUsersRequest,
            GetAllUsersResponse
        >(new GetAllUsersRequest());

        var request = new DeleteUserRequest { Id = getAllUsersResponse!.Users.ElementAt(1).Id };

        #endregion

        #region Act

        var response = await _client.DELETEAsync<DeleteUserEndpoint, DeleteUserRequest>(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        #endregion
    }
}
