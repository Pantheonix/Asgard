namespace Api.Features.Users.Delete;

public class DeleteUserSummary : Summary<DeleteUserEndpoint>
{
    public DeleteUserSummary()
    {
        Summary = "Delete a user";
        Description = "Delete a user by id only if the user is an admin";
        ExampleRequest = new DeleteUserRequest { Id = Guid.NewGuid() };
        Response(204, "User deleted successfully");
        Response<ErrorResponse>(404, "User not found");
        Response<ErrorResponse>(401, "Unauthorized access");
        Response<ErrorResponse>(403, "Forbidden access");
        Response<ErrorResponse>(500, "Internal server error");
    }
}
