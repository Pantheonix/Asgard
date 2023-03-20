namespace Api.Features.Users.Update;

public class UpdateUserSummary : Summary<UpdateUserEndpoint>
{
    public UpdateUserSummary()
    {
        Summary = "Update a user";
        Description = "Update a user by id";
        ExampleRequest = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            Username = "Test",
            Email = "test@gmail.com",
            Fullname = "Test User",
            Bio = "Test user bio"
        };
        Response<UpdateUserResponse>(
            200,
            "User updated successfully",
            example: new()
            {
                Id = Guid.NewGuid(),
                Username = "Test",
                Email = "test@gmail.com",
                Fullname = "Test User",
                Bio = "Test user bio",
                ProfilePictureUrl = "https://localhost:5001/api/images/6D31153C-C832-4530-8B93-B99CF657828C.png"
            }
        );
        Response<ErrorResponse>(400, "Validation failure");
        Response<ErrorResponse>(404, "User not found");
        Response<ErrorResponse>(401, "Unauthorized access");
        Response<ErrorResponse>(500, "Internal server error");
    }
}
