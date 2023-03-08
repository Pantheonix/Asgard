namespace Api.Auth.Register;

public class RegisterRequest { }

public class Validator : Validator<RegisterRequest>
{
    public Validator() { }
}

public class RegisterResponse
{
    public string Message { get; set; } = String.Empty;
}
