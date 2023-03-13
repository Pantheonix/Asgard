namespace Api.Features.Users.Update;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public override void Configure()
    {
        Put("{id}");
        Group<UsersGroup>();
    }

    public override Task HandleAsync(Request req, CancellationToken ct)
    {
        return SendOkAsync(Response, ct);
    }
}