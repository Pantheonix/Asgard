namespace Tests.Integration.Api.Features.Auth;

public class LoginEndpointTests : IClassFixture<ApiWebFactory>
{
    #region SetUp

    private readonly ApiWebFactory _apiWebFactory;
    private readonly HttpClient _client;

    private readonly Faker<ApplicationUser> _applicationUserFaker = new Faker<ApplicationUser>()
        .RuleFor(rule => rule.UserName, faker => faker.Internet.UserName().ClampLength(3))
        .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
        .RuleFor(rule => rule.Fullname, faker => faker.Internet.UserName().ClampLength(0, 50))
        .RuleFor(rule => rule.Bio, faker => faker.Lorem.Sentence().ClampLength(0, 300));

    private readonly Faker<LoginUserRequest> _loginUserRequestFaker = new Faker<LoginUserRequest>()
        .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
        .RuleFor(rule => rule.Password, faker => faker.Internet.Password());

    public LoginEndpointTests(ApiWebFactory apiWebFactory)
    {
        _apiWebFactory = apiWebFactory;
        _client = apiWebFactory.CreateClient();
    }

    #endregion

    [Fact]
    public async Task GivenValidUserCredentials_WhenLoggingIn_ThenReturnsOk()
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

        var loginRequest = _loginUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Email, applicationUser.Email)
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        #endregion

        #region Act

        var (response, result) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            LoginUserResponse
        >(loginRequest);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Username.Should().Be(applicationUser.UserName);
        result.Email.Should().Be(applicationUser.Email);
        result.Fullname.Should().Be(applicationUser.Fullname);
        result.Bio.Should().Be(applicationUser.Bio);
        result.ProfilePictureUrl.Should().NotBeNullOrWhiteSpace();
        result.Token.Should().NotBeNullOrWhiteSpace();

        #endregion
    }

    [Fact]
    public async Task GivenInvalidUserCredentials_WhenLoggingIn_ThenReturnsBadRequest()
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

        const string invalidPassword = "P@ssw0rd!1";
        var loginRequest = _loginUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Email, applicationUser.Email)
            .RuleFor(rule => rule.Password, invalidPassword)
            .Generate();

        #endregion

        #region Act

        var (response, result) = await _client.POSTAsync<
            LoginUserEndpoint,
            LoginUserRequest,
            ErrorResponse
        >(loginRequest);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result.Should().NotBeNull();
        result!.Errors.Should().NotBeEmpty();

        #endregion
    }
}
