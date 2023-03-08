namespace Api.Auth.Register;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse, Mapper>
{
    public override void Configure()
    {
        Post("/api/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        await SendOkAsync(
            new RegisterResponse { Message = "Method not implemented yet" },
            cancellation: ct
        );
    }
}
