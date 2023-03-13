namespace Api.Features.Users;

public sealed class UsersGroup : Group
{
    public UsersGroup()
    {
        Configure(
            "users",
            ep =>
            {
                ep.Description(builder => builder.RequireAuthorization());
            }
        );
    }
}
