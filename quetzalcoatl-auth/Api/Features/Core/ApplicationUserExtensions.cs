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

public enum SortUsersBy
{
    NameAsc,
    NameDesc,
    EmailAsc,
    EmailDesc,
}

public static class SortUsersByExtensions
{
    public static IEnumerable<UserDto> SortUsers(
        this IEnumerable<UserDto> query,
        SortUsersBy sortBy
    )
    {
        return sortBy switch
        {
            SortUsersBy.NameAsc => query.OrderBy(user => user.Username),
            SortUsersBy.NameDesc => query.OrderByDescending(user => user.Username),
            SortUsersBy.EmailAsc => query.OrderBy(user => user.Email),
            SortUsersBy.EmailDesc => query.OrderByDescending(user => user.Email),
            _ => query.OrderBy(user => user.Username),
        };
    }
}