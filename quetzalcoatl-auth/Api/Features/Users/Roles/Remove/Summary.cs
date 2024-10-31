namespace Api.Features.Users.Roles.Remove;

public class RemoveRoleSummary : Summary<RemoveRoleEndpoint>
{
    public RemoveRoleSummary()
    {
        Summary = "Remove a role from a user";
        Description = "Remove a role from a user by id";
        ExampleRequest = new RemoveRoleRequest
        {
            Id = Guid.NewGuid(),
            Role = ApplicationRole.Proposer.ToString()
        };
        Response<UserDto>(
            200,
            "Role removed successfully",
            example: new()
            {
                Id = Guid.NewGuid(),
                Username = "Test",
                Email = "test@gmail.com",
                Fullname = "Test User",
                Bio = "Test user bio",
                ProfilePictureId = Guid.NewGuid(),
                Roles = new List<string>()
            }
        );
        Response<ErrorResponse>(404, "User not found");
        Response<ErrorResponse>(401, "Unauthorized access");
        Response<ErrorResponse>(500, "Internal server error");
    }
}
