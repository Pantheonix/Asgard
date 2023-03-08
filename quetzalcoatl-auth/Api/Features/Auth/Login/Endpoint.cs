namespace Api.Auth.Login;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse, Mapper>
{
    public override void Configure()
    {
        Post("/api/login");
        AllowAnonymous();
    }

    public override Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        return SendOkAsync(Response, ct);
    }
}
