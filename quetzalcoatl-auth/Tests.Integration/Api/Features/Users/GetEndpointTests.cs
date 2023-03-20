namespace Tests.Integration.Api.Features.Users;

public class GetEndpointTests : IClassFixture<ApiWebFactory>
{
    #region SetUp

    private readonly ApiWebFactory _apiWebFactory;
    private readonly HttpClient _client;

    private readonly Faker<ApplicationUser> _applicationUserFaker = new Faker<ApplicationUser>()
        .RuleFor(rule => rule.UserName, faker => faker.Internet.UserName().ClampLength(3))
        .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
        .RuleFor(rule => rule.Fullname, faker => faker.Internet.UserName().ClampLength(0, 50))
        .RuleFor(rule => rule.Bio, faker => faker.Lorem.Sentence().ClampLength(0, 300));

    public GetEndpointTests(ApiWebFactory apiWebFactory)
    {
        _apiWebFactory = apiWebFactory;
        _client = apiWebFactory.CreateClient();
    }

    #endregion

    [Fact]
    public async Task GivenAnonymousUser_WhenGettingUser_ThenReturnsUnauthorized()
    {
        #region Arrange

        using var scope = _apiWebFactory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var profilePictureData = await ImageHelpers.GetImageAsByteArrayAsync(
            "https://picsum.photos/200"
        );
        var profilePicture = new Picture { Data = profilePictureData };

        var applicationUser = _applicationUserFaker
            .Clone()
            .RuleFor(rule => rule.ProfilePicture, profilePicture)
            .Generate();

        const string validPassword = "P@ssw0rd!";
        await userManager.CreateAsync(applicationUser, validPassword);

        var request = new GetUserRequest { Id = applicationUser.Id };

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

        using var scope = _apiWebFactory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var profilePictureData = await ImageHelpers.GetImageAsByteArrayAsync(
            "https://picsum.photos/200"
        );
        var profilePicture = new Picture { Data = profilePictureData };

        var applicationUser = _applicationUserFaker
            .Clone()
            .RuleFor(rule => rule.ProfilePicture, profilePicture)
            .Generate();

        const string validPassword = "P@ssw0rd!";
        await userManager.CreateAsync(applicationUser, validPassword);

        var loginUserRequest = new LoginUserRequest
        {
            Email = applicationUser.Email!,
            Password = validPassword
        };

        var (_, loginResult) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            LoginUserResponse
        >(loginUserRequest);

        var token = loginResult!.Token;

        var request = new GetUserRequest { Id = applicationUser.Id };

        var expectedResponse = new GetUserResponse
        {
            Id = applicationUser.Id,
            Username = applicationUser.UserName!,
            Email = applicationUser.Email!,
            Fullname = applicationUser.Fullname,
            Bio = applicationUser.Bio,
            ProfilePictureUrl = applicationUser.GetProfilePictureUrl(
                ProfilePictureConstants.BaseUrl,
                ProfilePictureConstants.EndpointUrl,
                ProfilePictureConstants.Extension
            )
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

        using var scope = _apiWebFactory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var profilePictureData = await ImageHelpers.GetImageAsByteArrayAsync(
            "https://picsum.photos/200"
        );
        var profilePicture = new Picture { Data = profilePictureData };

        var applicationUser = _applicationUserFaker
            .Clone()
            .RuleFor(rule => rule.ProfilePicture, profilePicture)
            .Generate();

        const string validPassword = "P@ssw0rd!";
        await userManager.CreateAsync(applicationUser, validPassword);

        var loginUserRequest = new LoginUserRequest
        {
            Email = applicationUser.Email!,
            Password = validPassword
        };

        var (_, loginResult) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            LoginUserResponse
        >(loginUserRequest);

        var token = loginResult!.Token;

        var request = new GetUserRequest { Id = Guid.NewGuid() };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.GETAsync<GetUserEndpoint, GetUserRequest>(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        #endregion
    }
}
