namespace Api.Features.Auth.Register;

public class RegisterUserSummary : Summary<RegisterUserEndpoint>
{
    public RegisterUserSummary()
    {
        Summary = "Register a user";
        Description = "Register a new user with username, email and password";
        ExampleRequest = new RegisterUserRequest
        {
            Username = "Test",
            Email = "test@gmail.com",
            Password = "Password123!"
        };
        Response<RegisterUserResponse>(
            201,
            "User created/registered successfully",
            example: new()
            {
                Username = "Test",
                Email = "test@gmail.com",
                Token = "JWT Access Token"
            }
        );
        Response<ErrorResponse>(400, "Validation failure");
        Response<ErrorResponse>(500, "Internal server error");
    }
}
