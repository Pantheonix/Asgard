namespace Api.Features.Auth.Login;

public class LoginRequest { }

public class Validator : Validator<LoginRequest>
{
    public Validator() { }
}

public class LoginResponse
{
    public string Message => "This endpoint hasn't been implemented yet!";
}
