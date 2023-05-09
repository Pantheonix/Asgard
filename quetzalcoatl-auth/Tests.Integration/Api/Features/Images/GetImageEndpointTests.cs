namespace Tests.Integration.Api.Features.Images;

public class GetImageEndpointTests : IClassFixture<ApiWebFactory>
{
    #region SetUp

    private readonly ApiWebFactory _apiWebFactory;
    private readonly HttpClient _client;

    private readonly Faker<ApplicationUser> _applicationUserFaker = new Faker<ApplicationUser>()
        .RuleFor(rule => rule.UserName, faker => faker.Internet.UserName().ClampLength(3))
        .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
        .RuleFor(rule => rule.Fullname, faker => faker.Internet.UserName().ClampLength(0, 50))
        .RuleFor(rule => rule.Bio, faker => faker.Lorem.Sentence().ClampLength(0, 300));

    public GetImageEndpointTests(ApiWebFactory apiWebFactory)
    {
        _apiWebFactory = apiWebFactory;
        _client = apiWebFactory.CreateClient();
    }

    #endregion

    [Fact]
    public async Task GivenAnonymousUser_WhenGettingImage_ThenReturnsUnauthorized()
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

        var request = new GetImageRequest { Id = applicationUser.ProfilePicture!.Id };

        #endregion

        #region Act

        var response = await _client.GETAsync<GetImageEndpoint, GetImageRequest>(request);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndNonExistingImageId_WhenGettingImage_ThenReturnsNotFound()
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

        var request = new GetImageRequest { Id = Guid.NewGuid() };

        var loginUserRequest = new LoginUserRequest
        {
            Email = applicationUser.Email!,
            Password = validPassword
        };

        var (loginHttpResponse, _) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            UserTokenResponse
        >(loginUserRequest);

        var token = TokenHelpers.ExtractTokenFromResponse(loginHttpResponse);
        
        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.GETAsync<GetImageEndpoint, GetImageRequest>(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndExistingImageId_WhenGettingImage_ThenReturnsOk()
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

        var request = new GetImageRequest { Id = applicationUser.ProfilePicture!.Id };

        var loginUserRequest = new LoginUserRequest
        {
            Email = applicationUser.Email!,
            Password = validPassword
        };

        var (loginHttpResponse, _) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            UserTokenResponse
        >(loginUserRequest);

        var token = TokenHelpers.ExtractTokenFromResponse(loginHttpResponse);

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.GETAsync<GetImageEndpoint, GetImageRequest>(request);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("image/jpeg");
        response.Content.Headers.ContentLength.Should().BeGreaterThan(0);

        #endregion
    }
}
