namespace Api.Features.Images.Get;

public class GetImageSummary : Summary<GetImageEndpoint>
{
    public GetImageSummary()
    {
        Summary = "Get a user's profile picture";
        Description = "Get a user's profile picture by their ID";
        ExampleRequest = new GetImageRequest { Id = Guid.NewGuid() };
        Response(200, "User's profile picture retrieved successfully");
        Response<ErrorResponse>(404, "Not authorized");
        Response<ErrorResponse>(500, "Internal server error");
    }
}
