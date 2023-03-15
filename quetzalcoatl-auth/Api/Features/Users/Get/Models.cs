namespace Api.Features.Users.Get;

public class GetUserRequest
{
    [BindFrom("username")]
    public string Username { get; set; } = string.Empty;
}

public class GetUserResponse
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
