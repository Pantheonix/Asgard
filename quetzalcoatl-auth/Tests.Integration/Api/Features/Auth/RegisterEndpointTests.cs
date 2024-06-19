using System.Net.Http.Formatting;

namespace Tests.Integration.Api.Features.Auth;

public class RegisterEndpointTests : IClassFixture<ApiWebFactory>
{
    #region SetUp

    private readonly HttpClient _client;

    private readonly Faker<RegisterUserRequest> _registerUserRequestFaker =
        new Faker<RegisterUserRequest>()
            .RuleFor(rule => rule.Username, faker => faker.Internet.UserName().ClampLength(3))
            .RuleFor(rule => rule.Email, faker => faker.Internet.Email())
            .RuleFor(rule => rule.Password, faker => faker.Internet.Password())
            .RuleFor(rule => rule.Fullname, faker => faker.Internet.UserName().ClampLength(0, 50))
            .RuleFor(rule => rule.Bio, faker => faker.Lorem.Sentence().ClampLength(0, 300));

    public RegisterEndpointTests(ApiWebFactory apiWebFactory)
    {
        _client = apiWebFactory.CreateClient();
    }

    #endregion

    [Fact]
    public async Task GivenValidUser_WhenRegistering_ThenReturnsCreated()
    {
        #region Arrange

        const string validPassword = "P@ssw0rd!";
        var request = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Password, validPassword)
            .Generate();

        request.ProfilePicture = await ImageHelpers.GetImageAsFormFileAsync(
            "https://picsum.photos/200",
            "demo.jpg"
        );

        _client
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client
            .DefaultRequestHeaders
            .TryAddWithoutValidation("Content-Type", "multipart/form-data");

        var requestForm = new MultipartFormDataContent();

        requestForm.Add(new StringContent(request.Username), nameof(request.Username));
        requestForm.Add(new StringContent(request.Email), nameof(request.Email));
        requestForm.Add(new StringContent(request.Password), nameof(request.Password));
        requestForm.Add(new StringContent(request.Fullname!), nameof(request.Fullname));
        requestForm.Add(new StringContent(request.Bio!), nameof(request.Bio));

        var profilePictureContent = new StreamContent(request.ProfilePicture.OpenReadStream());
        profilePictureContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            MediaTypeNames.Image.Jpeg
        );
        requestForm.Add(
            profilePictureContent,
            nameof(request.ProfilePicture),
            request.ProfilePicture.FileName
        );

        #endregion

        #region Act

        var response = await _client.PostAsync("/api/auth/register", requestForm);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        // check if AccessToken and RefreshToken cookies exist
        response.Headers.TryGetValues("Set-Cookie", out var tokenCookiesValues).Should().BeTrue();
        tokenCookiesValues.Should().HaveCount(2);

        var result = await response.Content.ReadAsAsync<UserTokenResponse>();

        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Username.Should().Be(request.Username);
        result.Email.Should().Be(request.Email);
        result.Fullname.Should().Be(request.Fullname);
        result.Bio.Should().Be(request.Bio);
        result.ProfilePictureId.Should().NotBeNull();

        #endregion
    }

    [Fact]
    public async Task GivenInvalidUser_WhenRegistering_ThenReturnsBadRequest()
    {
        #region Arrange

        const string invalidPassword = "invalidpassword";
        var request = _registerUserRequestFaker
            .Clone()
            .RuleFor(rule => rule.Password, invalidPassword)
            .Generate();

        request.ProfilePicture = await ImageHelpers.GetImageAsFormFileAsync(
            "https://picsum.photos/id/237/200/300",
            "demo.jpg"
        );

        _client
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client
            .DefaultRequestHeaders
            .TryAddWithoutValidation("Content-Type", "multipart/form-data");

        var requestForm = new MultipartFormDataContent();

        requestForm.Add(new StringContent(request.Username), nameof(request.Username));
        requestForm.Add(new StringContent(request.Email), nameof(request.Email));
        requestForm.Add(new StringContent(request.Password), nameof(request.Password));
        requestForm.Add(new StringContent(request.Fullname!), nameof(request.Fullname));
        requestForm.Add(new StringContent(request.Bio!), nameof(request.Bio));
        requestForm.Add(
            new StreamContent(request.ProfilePicture.OpenReadStream()),
            nameof(request.ProfilePicture),
            request.ProfilePicture.FileName
        );

        #endregion

        #region Act

        var response = await _client.PostAsync("/api/auth/register", requestForm);

        #endregion

        #region Assert

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        // check if AccessToken and RefreshToken cookies exist
        response.Headers.TryGetValues("Set-Cookie", out _).Should().BeFalse();

        var formatters = new MediaTypeFormatterCollection();
        formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/problem+json"));
        
        var result = await response.Content.ReadAsAsync<ErrorResponse>(formatters: formatters);

        result.Should().NotBeNull();
        result!.Errors.Keys.Select(r => r.ToUpper())
            .Should().Contain(nameof(request.Password).ToUpper());
        result.Errors.Keys.Select(r => r.ToUpper())
            .Should().Contain(nameof(request.ProfilePicture).ToUpper());

        #endregion
    }
}
