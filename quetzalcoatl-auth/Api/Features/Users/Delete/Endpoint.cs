namespace Api.Features.Users.Delete;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public override void Configure()
    {
        Delete("{id}");
        Group<UsersGroup>();
    }

    public override Task HandleAsync(Request req, CancellationToken ct)
    {
        return SendOkAsync(Response, ct);
    }
}