namespace Api.Features.Core;

public static class ApplicationUserExtensions
{
    public static string GetProfilePictureUrl(
        this ApplicationUser applicationUser,
        string baseUrl,
        string endpointUrl,
        string extension
    )
    {
        return $"{baseUrl}/{endpointUrl}/{applicationUser.ProfilePicture.Id.ToString()}.{extension}";
    }
}
