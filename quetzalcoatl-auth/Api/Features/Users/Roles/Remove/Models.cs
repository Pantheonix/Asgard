namespace Api.Features.Users.Roles.Remove;

public class RemoveRoleRequest
{
    [FromRoute]
    public Guid Id { get; set; } = Guid.Empty;

    public string Role { get; set; } = string.Empty;
}