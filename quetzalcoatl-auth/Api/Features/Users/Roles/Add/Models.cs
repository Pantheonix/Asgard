namespace Api.Features.Users.Roles.Add;

public class AddRoleRequest
{
    [FromRoute]
    public Guid Id { get; set; } = Guid.Empty;

    public string Role { get; set; } = string.Empty;
}