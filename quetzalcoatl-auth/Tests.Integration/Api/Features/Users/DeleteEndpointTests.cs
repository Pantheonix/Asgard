namespace Tests.Integration.Api.Features.Users;

public class DeleteEndpointTests : IClassFixture<ApiWebFactory>
{
    #region SetUp

    private readonly ApiWebFactory _apiWebFactory;
    private readonly HttpClient _client;

    private readonly Faker<ApplicationUser> _applicationUserFaker = new Faker<ApplicationUser>()
        .RuleFor(rule => rule.UserName, faker => faker.Internet.UserName().ClampLength(3))
        .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
        .RuleFor(rule => rule.Fullname, faker => faker.Internet.UserName().ClampLength(0, 50))
        .RuleFor(rule => rule.Bio, faker => faker.Lorem.Sentence().ClampLength(0, 300));

    public DeleteEndpointTests(ApiWebFactory apiWebFactory)
    {
        _apiWebFactory = apiWebFactory;
        _client = apiWebFactory.CreateClient();
    }

    #endregion

    [Fact]
    public async Task GivenAnonymousUser_WhenDeletingUser_ThenReturnsUnauthorized()
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
        var users = await userManager.Users.ToListAsync();

        var request = new DeleteUserRequest { Id = applicationUser.Id };

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
    public async Task GivenAuthorizedUserButNonAdmin_WhenDeletingUser_ThenReturnsForbidden()
    {
        #region Arrange

        using var scope = _apiWebFactory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var profilePictureData = await ImageHelpers.GetImageAsByteArrayAsync(
            "https://picsum.photos/200"
        );

        var users = new List<ApplicationUser>();

        const string validPassword = "P@ssw0rd!";

        for (var i = 0; i < 3; i++)
        {
            var profilePicture = new Picture { Data = profilePictureData };
            var applicationUser = _applicationUserFaker
                .Clone()
                .RuleFor(rule => rule.ProfilePicture, profilePicture)
                .Generate();

            users.Add(applicationUser);

            await userManager.CreateAsync(applicationUser, validPassword);
        }

        var loginUserRequest = new LoginUserRequest
        {
            Email = users.ElementAt(0).Email!,
            Password = validPassword
        };

        var (loginHttpResponse, _) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            UserTokenResponse
        >(loginUserRequest);

        var token = TokenHelpers.ExtractTokenFromResponse(loginHttpResponse);

        var request = new DeleteUserRequest { Id = Guid.NewGuid() };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.DELETEAsync<DeleteUserEndpoint, DeleteUserRequest>(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndIdOfNonExistingUser_WhenDeletingUser_ThenReturnsNotFound()
    {
        #region Arrange

        using var scope = _apiWebFactory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope
            .ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await roleManager.CreateAsync(new IdentityRole<Guid>(ApplicationRole.Admin.ToString()));

        var profilePictureData = await ImageHelpers.GetImageAsByteArrayAsync(
            "https://picsum.photos/200"
        );

        var users = new List<ApplicationUser>();

        const string validPassword = "P@ssw0rd!";

        for (var i = 0; i < 3; i++)
        {
            var profilePicture = new Picture { Data = profilePictureData };
            var applicationUser = _applicationUserFaker
                .Clone()
                .RuleFor(rule => rule.ProfilePicture, profilePicture)
                .Generate();

            users.Add(applicationUser);

            await userManager.CreateAsync(applicationUser, validPassword);
        }

        await userManager.AddToRoleAsync(users.ElementAt(0), ApplicationRole.Admin.ToString());

        var loginUserRequest = new LoginUserRequest
        {
            Email = users.ElementAt(0).Email!,
            Password = validPassword
        };

        var (loginHttpResponse, _) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            UserTokenResponse
        >(loginUserRequest);

        var token = TokenHelpers.ExtractTokenFromResponse(loginHttpResponse);

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

        using var scope = _apiWebFactory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope
            .ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await roleManager.CreateAsync(new IdentityRole<Guid>(ApplicationRole.Admin.ToString()));

        var profilePictureData = await ImageHelpers.GetImageAsByteArrayAsync(
            "https://picsum.photos/200"
        );

        var users = new List<ApplicationUser>();

        const string validPassword = "P@ssw0rd!";
        for (var i = 0; i < 3; i++)
        {
            var profilePicture = new Picture { Data = profilePictureData };
            var applicationUser = _applicationUserFaker
                .Clone()
                .RuleFor(rule => rule.ProfilePicture, profilePicture)
                .Generate();

            users.Add(applicationUser);

            await userManager.CreateAsync(applicationUser, validPassword);
        }

        await userManager.AddToRoleAsync(users.ElementAt(0), ApplicationRole.Admin.ToString());

        var loginUserRequest = new LoginUserRequest
        {
            Email = users.ElementAt(0).Email!,
            Password = validPassword
        };

        var (loginHttpResponse, _) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            UserTokenResponse
        >(loginUserRequest);

        var token = TokenHelpers.ExtractTokenFromResponse(loginHttpResponse);

        var request = new DeleteUserRequest { Id = users.ElementAt(1).Id };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.DELETEAsync<DeleteUserEndpoint, DeleteUserRequest>(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        #endregion
    }
}
