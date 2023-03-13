namespace Api.Features.Users.GetAll;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public override void Configure()
    {
        Get("/");
        Group<UsersGroup>();
    }

    public override Task HandleAsync(Request req, CancellationToken ct)
    {
        return SendOkAsync(Response, ct);
    }
}
