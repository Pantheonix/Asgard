namespace Tests.Integration.Api.Features.Users;

public class GetAllEndpointTests : IClassFixture<ApiWebFactory>
{
    #region SetUp

    private readonly HttpClient _client;

    private readonly Faker<RegisterUserRequest> _registerUserRequestFaker =
        new Faker<RegisterUserRequest>()
            .RuleFor(rule => rule.Username, faker => faker.Internet.UserName().ClampLength(3))
            .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
            .RuleFor(rule => rule.Password, faker => faker.Internet.Password());

    public GetAllEndpointTests(ApiWebFactory apiWebFactory)
    {
        _client = apiWebFactory.CreateClient();
    }

    #endregion

    [Fact]
    public async Task GivenAuthorizedUser_WhenGettingAllUsers_ThenReturnsOk()
    {
        #region Arrange

        var users = new List<UserDto>();

        const string validPassword = "P@ssw0rd!";
        for (var i = 0; i < 3; i++)
        {
            var registerUserRequest = _registerUserRequestFaker
                .Clone()
                .RuleFor(rule => rule.Password, $"{validPassword}{i}")
                .Generate();

            users.Add(MapUserDtoFrom(registerUserRequest));

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

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var (response, result) = await _client.GETAsync<
            GetAllUsersEndpoint,
            GetAllUsersRequest,
            GetAllUsersResponse
        >(new GetAllUsersRequest());

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Should().NotBeNull();
        result!.Users
            .Select(user =>
            {
                user.Id = Guid.Empty;
                return user;
            })
            .Should()
            .BeEquivalentTo(users);

        #endregion
    }

    [Fact]
    public async Task GivenAnonymousUser_WhenGettingAllUsers_ThenReturnsUnauthorized()
    {
        #region Arrange

        const string validPassword = "P@ssw0rd!";
        for (var i = 0; i < 3; i++)
        {
            var registerUserRequest = _registerUserRequestFaker
                .Clone()
                .RuleFor(rule => rule.Password, $"{validPassword}{i}")
                .Generate();

            await _client.POSTAsync<
                RegisterUserEndpoint,
                RegisterUserRequest,
                RegisterUserResponse
            >(registerUserRequest);
        }

        #endregion

        #region Act

        var response = await _client.GETAsync<GetAllUsersEndpoint, GetAllUsersRequest>(
            new GetAllUsersRequest()
        );

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        #endregion
    }

    private static UserDto MapUserDtoFrom(RegisterUserRequest request)
    {
        return new UserDto { Username = request.Username, Email = request.Email };
    }
}
