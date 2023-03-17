namespace Api.Features.Users.Get;

public class GetUserRequest
{
    [BindFrom("id")]
    public Guid Id { get; set; } = Guid.Empty;
}

public class GetUserResponse
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
