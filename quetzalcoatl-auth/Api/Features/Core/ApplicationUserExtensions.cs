namespace Api.Features.Core;

public static class ApplicationUserExtensions
{
    public static string? GetProfilePictureUrl(
        this ApplicationUser applicationUser,
        string baseUrl,
        string endpointUrl,
        string extension
    )
    {
        return applicationUser.ProfilePicture is not null
            ? $"{baseUrl}/{endpointUrl}/{applicationUser.ProfilePicture.Id.ToString()}.{extension}"
            : null;
    }
}
