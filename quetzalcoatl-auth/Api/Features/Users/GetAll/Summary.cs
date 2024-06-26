namespace Api.Features.Users.GetAll;

public class GetAllUsersSummary : Summary<GetAllUsersEndpoint>
{
    public GetAllUsersSummary()
    {
        Summary = "Get all users";
        Description = "Get all users";
        ExampleRequest = new GetAllUsersRequest();
        Response<GetAllUsersResponse>(
            200,
            "Users retrieved successfully",
            example: new()
            {
                Users = new[]
                {
                    new UserDto
                    {
                        Id = Guid.NewGuid(),
                        Username = "Test1",
                        Email = "test1@gmail.com",
                        Fullname = "Test 1",
                        Bio = "Test 1 bio",
                        ProfilePictureId = Guid.NewGuid(),
                    },
                    new UserDto
                    {
                        Id = Guid.NewGuid(),
                        Username = "Test2",
                        Email = "test2@gmail.com",
                        Fullname = "Test 2",
                        Bio = "Test 2 bio",
                        ProfilePictureId = Guid.NewGuid(),
                    },
                    new UserDto
                    {
                        Id = Guid.NewGuid(),
                        Username = "Test3",
                        Email = "test3@gmail.com",
                        Fullname = "Test 3",
                        Bio = "Test 3 bio",
                        ProfilePictureId = Guid.NewGuid(),
                    }
                }
            }
        );
        Response<ErrorResponse>(401, "Unauthorized access");
        Response<ErrorResponse>(500, "Internal server error");
    }
}
