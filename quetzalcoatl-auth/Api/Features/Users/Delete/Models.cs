namespace Api.Features.Users.Delete;

public class DeleteUserRequest
{
    [BindFrom("id")]
    public Guid Id { get; set; } = Guid.Empty;
}
