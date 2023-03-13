namespace Api.Features.Auth;

public sealed class AuthenticationGroup : Group
{
    public AuthenticationGroup()
    {
        Configure(
            "auth",
            ep =>
            {
                ep.Description(builder => builder.AllowAnonymous());
            }
        );
    }
}
