namespace Tests.Integration.Api.Features.Users;

public class GetAllEndpointTests : IClassFixture<ApiWebFactory>
{
    #region SetUp

    private readonly ApiWebFactory _apiWebFactory;
    private readonly HttpClient _client;

    private readonly Faker<ApplicationUser> _applicationUserFaker = new Faker<ApplicationUser>()
        .RuleFor(rule => rule.UserName, faker => faker.Internet.UserName().ClampLength(3))
        .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
        .RuleFor(rule => rule.Fullname, faker => faker.Internet.UserName().ClampLength(0, 50))
        .RuleFor(rule => rule.Bio, faker => faker.Lorem.Sentence().ClampLength(0, 300));

    public GetAllEndpointTests(ApiWebFactory apiWebFactory)
    {
        _apiWebFactory = apiWebFactory;
        _client = apiWebFactory.CreateClient();
    }

    #endregion

    [Fact]
    public async Task GivenAuthorizedUser_WhenGettingAllUsers_ThenReturnsOk()
    {
        #region Arrange

        using var scope = _apiWebFactory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        var existingUsers = await userManager.Users.ToListAsync();
        await userManager.DeleteAsync(existingUsers.ElementAt(0));

        var profilePictureData = await ImageHelpers.GetImageAsByteArrayAsync(
            "https://picsum.photos/200"
        );

        var users = new List<UserDto>();

        const string validPassword = "P@ssw0rd!";
        for (var i = 0; i < 3; i++)
        {
            var profilePicture = new Picture { Data = profilePictureData };
            var applicationUser = _applicationUserFaker
                .Clone()
                .RuleFor(rule => rule.ProfilePicture, profilePicture)
                .Generate();

            await userManager.CreateAsync(applicationUser, validPassword);

            users.Add(MapUserDtoFrom(applicationUser));
        }

        var loginUserRequest = new LoginUserRequest
        {
            Email = users.ElementAt(0).Email,
            Password = validPassword
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
        result!.Users.Should().BeEquivalentTo(users);

        #endregion
    }

    [Fact]
    public async Task GivenAnonymousUser_WhenGettingAllUsers_ThenReturnsUnauthorized()
    {
        #region Arrange

        using var scope = _apiWebFactory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var profilePictureData = await ImageHelpers.GetImageAsByteArrayAsync(
            "https://picsum.photos/200"
        );

        const string validPassword = "P@ssw0rd!";
        for (var i = 0; i < 3; i++)
        {
            var profilePicture = new Picture { Data = profilePictureData };
            var applicationUser = _applicationUserFaker
                .Clone()
                .RuleFor(rule => rule.ProfilePicture, profilePicture)
                .Generate();

            await userManager.CreateAsync(applicationUser, validPassword);
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

    private static UserDto MapUserDtoFrom(ApplicationUser appUser)
    {
        return new UserDto
        {
            Id = appUser.Id,
            Username = appUser.UserName!,
            Email = appUser.Email!,
            Fullname = appUser.Fullname,
            Bio = appUser.Bio,
            ProfilePictureUrl = appUser.GetProfilePictureUrl(
                ProfilePictureConstants.BaseUrl,
                ProfilePictureConstants.EndpointUrl,
                ProfilePictureConstants.Extension
            )
        };
    }
}
