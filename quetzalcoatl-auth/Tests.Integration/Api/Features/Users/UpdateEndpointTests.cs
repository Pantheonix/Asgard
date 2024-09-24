namespace Tests.Integration.Api.Features.Users;

public class UpdateEndpointTests : IClassFixture<ApiWebFactory>
{
    #region SetUp

    private readonly ApiWebFactory _apiWebFactory;
    private readonly HttpClient _client;

    private readonly Faker<ApplicationUser> _applicationUserFaker = new Faker<ApplicationUser>()
        .RuleFor(rule => rule.UserName, faker => faker.Internet.UserName().ClampLength(3))
        .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
        .RuleFor(rule => rule.Fullname, faker => faker.Internet.UserName().ClampLength(0, 50))
        .RuleFor(rule => rule.Bio, faker => faker.Lorem.Sentence().ClampLength(0, 300));

    public UpdateEndpointTests(ApiWebFactory apiWebFactory)
    {
        _apiWebFactory = apiWebFactory;
        _client = apiWebFactory.CreateClient();
    }

    #endregion

    [Fact]
    public async Task GivenAnonymousUser_WhenUpdatingUser_ThenReturnsUnauthorized()
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

        var updateProfilePictureFormFile = await ImageHelpers.GetImageAsFormFileAsync(
            "https://picsum.photos/200/300",
            "demo.jpg"
        );

        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        _client.DefaultRequestHeaders.TryAddWithoutValidation(
            "Content-Type",
            "multipart/form-data"
        );

        var requestForm = new MultipartFormDataContent();

        requestForm.Add(
            new StringContent($"{applicationUser.UserName!}-updated"),
            nameof(applicationUser.UserName)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Email!}-updated"),
            nameof(applicationUser.Email)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Fullname!}-updated"),
            nameof(applicationUser.Fullname)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Bio!}-updated"),
            nameof(applicationUser.Bio)
        );

        var profilePictureContent = new StreamContent(
            updateProfilePictureFormFile.OpenReadStream()
        );
        profilePictureContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            MediaTypeNames.Image.Jpeg
        );
        requestForm.Add(
            profilePictureContent,
            nameof(applicationUser.ProfilePicture),
            updateProfilePictureFormFile.FileName
        );

        #endregion

        #region Act

        var response = await _client.PutAsync($"/api/users/{applicationUser.Id}", requestForm);

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

        var updateProfilePictureFormFile = await ImageHelpers.GetImageAsFormFileAsync(
            "https://picsum.photos/200/300",
            "demo.jpg"
        );

        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        _client.DefaultRequestHeaders.TryAddWithoutValidation(
            "Content-Type",
            "multipart/form-data"
        );

        var requestForm = new MultipartFormDataContent();

        requestForm.Add(
            new StringContent($"{applicationUser.UserName!}-updated"),
            nameof(applicationUser.UserName)
        );
        requestForm.Add(new StringContent("invalid-email-updated"), nameof(applicationUser.Email));
        requestForm.Add(
            new StringContent($"{applicationUser.Fullname!}-updated"),
            nameof(applicationUser.Fullname)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Bio!}-updated"),
            nameof(applicationUser.Bio)
        );

        var profilePictureContent = new StreamContent(
            updateProfilePictureFormFile.OpenReadStream()
        );
        profilePictureContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            MediaTypeNames.Image.Jpeg
        );
        requestForm.Add(
            profilePictureContent,
            nameof(applicationUser.ProfilePicture),
            updateProfilePictureFormFile.FileName
        );

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

        var response = await _client.PutAsync($"/api/users/{applicationUser.Id}", requestForm);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadAsAsync<ErrorResponse>();

        result.Should().NotBeNull();
        result!.Errors.Keys.Should().Contain(nameof(applicationUser.Email));

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndRequestForUpdatingOtherUserThanSelf_WhenUpdatingUser_ThenReturnsForbidden()
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

        var updateProfilePictureFormFile = await ImageHelpers.GetImageAsFormFileAsync(
            "https://picsum.photos/200/300",
            "demo.jpg"
        );

        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        _client.DefaultRequestHeaders.TryAddWithoutValidation(
            "Content-Type",
            "multipart/form-data"
        );

        var requestForm = new MultipartFormDataContent();

        requestForm.Add(
            new StringContent($"{applicationUser.UserName!}-updated"),
            nameof(applicationUser.UserName)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Email!}-updated"),
            nameof(applicationUser.Email)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Fullname!}-updated"),
            nameof(applicationUser.Fullname)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Bio!}-updated"),
            nameof(applicationUser.Bio)
        );

        var profilePictureContent = new StreamContent(
            updateProfilePictureFormFile.OpenReadStream()
        );
        profilePictureContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            MediaTypeNames.Image.Jpeg
        );
        requestForm.Add(
            profilePictureContent,
            nameof(applicationUser.ProfilePicture),
            updateProfilePictureFormFile.FileName
        );

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

        var response = await _client.PutAsync(
            $"/api/users/{Guid.NewGuid().ToString()}",
            requestForm
        );

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndValidRequestForPartialUpdate_WhenUpdatingUser_ThenReturnsOk()
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

        var updateProfilePictureFormFile = await ImageHelpers.GetImageAsFormFileAsync(
            "https://picsum.photos/200/300",
            "demo.jpg"
        );

        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        _client.DefaultRequestHeaders.TryAddWithoutValidation(
            "Content-Type",
            "multipart/form-data"
        );

        var requestForm = new MultipartFormDataContent();

        requestForm.Add(
            new StringContent($"{applicationUser.UserName!}-updated"),
            nameof(applicationUser.UserName)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Fullname!}-updated"),
            nameof(applicationUser.Fullname)
        );

        var profilePictureContent = new StreamContent(
            updateProfilePictureFormFile.OpenReadStream()
        );
        profilePictureContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            MediaTypeNames.Image.Jpeg
        );
        requestForm.Add(
            profilePictureContent,
            nameof(applicationUser.ProfilePicture),
            updateProfilePictureFormFile.FileName
        );

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

        var expectedResponse = new UpdateUserResponse
        {
            Id = applicationUser.Id,
            Username = $"{applicationUser.UserName!}-updated",
            Email = applicationUser.Email!,
            Fullname = $"{applicationUser.Fullname!}-updated",
            Bio = applicationUser.Bio,
            ProfilePictureId = applicationUser.ProfilePicture?.Id
        };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.PutAsync($"/api/users/{applicationUser.Id}", requestForm);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadAsAsync<UpdateUserResponse>();

        result.Should().NotBeNull();
        result
            .Should()
            .BeEquivalentTo(expectedResponse, opt => opt.Excluding(r => r.ProfilePictureId));

        #endregion
    }

    [Fact]
    public async Task GivenAuthorizedUserAndValidRequest_WhenUpdatingUser_ThenReturnsOk()
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

        var updateProfilePictureFormFile = await ImageHelpers.GetImageAsFormFileAsync(
            "https://picsum.photos/200/300",
            "demo.jpg"
        );

        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        _client.DefaultRequestHeaders.TryAddWithoutValidation(
            "Content-Type",
            "multipart/form-data"
        );

        var requestForm = new MultipartFormDataContent();

        requestForm.Add(
            new StringContent($"{applicationUser.UserName!}-updated"),
            nameof(applicationUser.UserName)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Email!}-updated"),
            nameof(applicationUser.Email)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Fullname!}-updated"),
            nameof(applicationUser.Fullname)
        );
        requestForm.Add(
            new StringContent($"{applicationUser.Bio!}-updated"),
            nameof(applicationUser.Bio)
        );

        var profilePictureContent = new StreamContent(
            updateProfilePictureFormFile.OpenReadStream()
        );
        profilePictureContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            MediaTypeNames.Image.Jpeg
        );
        requestForm.Add(
            profilePictureContent,
            nameof(applicationUser.ProfilePicture),
            updateProfilePictureFormFile.FileName
        );

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

        var expectedResponse = new UpdateUserResponse
        {
            Id = applicationUser.Id,
            Username = $"{applicationUser.UserName!}-updated",
            Email = $"{applicationUser.Email!}-updated",
            Fullname = $"{applicationUser.Fullname!}-updated",
            Bio = $"{applicationUser.Bio!}-updated",
            ProfilePictureId = applicationUser.ProfilePicture?.Id
        };

        #endregion

        #region Act

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.PutAsync($"/api/users/{applicationUser.Id}", requestForm);

        _client.DefaultRequestHeaders.Remove("Authorization");

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadAsAsync<UpdateUserResponse>();

        result.Should().NotBeNull();
        result
            .Should()
            .BeEquivalentTo(expectedResponse, opt => opt.Excluding(r => r.ProfilePictureId));

        #endregion
    }
}
