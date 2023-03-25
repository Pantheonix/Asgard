namespace Api.Features.Users.Delete;

public class DeleteUserRequest
{
    public Guid Id { get; init; } = Guid.Empty;
}
