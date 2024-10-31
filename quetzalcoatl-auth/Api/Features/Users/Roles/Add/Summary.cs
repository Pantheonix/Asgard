namespace Api.Features.Users.Roles.Add;

public class AddRoleSummary : Summary<AddRoleEndpoint>
{
    public AddRoleSummary()
    {
        Summary = "Add a role to a user";
        Description = "Add a role to a user by id";
        ExampleRequest = new AddRoleRequest
        {
            Id = Guid.NewGuid(),
            Role = ApplicationRole.Proposer.ToString()
        };
        Response<UserDto>(
            200,
            "Role added successfully",
            example: new()
            {
                Id = Guid.NewGuid(),
                Username = "Test",
                Email = "test@gmail.com",
                Fullname = "Test User",
                Bio = "Test user bio",
                ProfilePictureId = Guid.NewGuid(),
                Roles = new List<string> { ApplicationRole.Proposer.ToString() }
            }
        );
        Response<ErrorResponse>(404, "User not found");
        Response<ErrorResponse>(401, "Unauthorized access");
        Response<ErrorResponse>(500, "Internal server error");
    }
}
