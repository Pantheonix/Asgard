namespace Api.Features.Users.Get;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public override void Configure()
    {
        Get("{id}");
        Group<UsersGroup>();
    }

    public override Task HandleAsync(Request req, CancellationToken ct)
    {
        return SendOkAsync(Response, ct);
    }
}
