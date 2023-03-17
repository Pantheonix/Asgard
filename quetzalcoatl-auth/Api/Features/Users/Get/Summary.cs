namespace Api.Features.Users.Get;

public class GetUserSummary : Summary<GetUserEndpoint>
{
    public GetUserSummary()
    {
        Summary = "Get a user";
        Description = "Get a user by id";
        ExampleRequest = new GetUserRequest { Id = Guid.NewGuid() };
        Response<GetUserResponse>(
            200,
            "User retrieved successfully",
            example: new()
            {
                Id = Guid.NewGuid(),
                Username = "Test",
                Email = "test@gmail.com"
            }
        );
        Response<ErrorResponse>(404, "User not found");
        Response<ErrorResponse>(401, "Unauthorized access");
        Response<ErrorResponse>(500, "Internal server error");
    }
}
