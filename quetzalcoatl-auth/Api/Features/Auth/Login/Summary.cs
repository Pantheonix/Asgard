namespace Api.Features.Auth.Login;

public class LoginUserSummary : Summary<LoginUserEndpoint>
{
    public LoginUserSummary()
    {
        Summary = "Login a user";
        Description = "Login a user with a username and password";
        ExampleRequest = new LoginUserRequest
        {
            Email = "test@gmail.com",
            Password = "Password123!"
        };
        Response<LoginUserResponse>(
            200,
            "User logged in successfully",
            example: new()
            {
                Username = "Test",
                Email = "test@gmail.com",
                Token = "JWT Access Token"
            }
        );
        Response<ErrorResponse>(400, "Invalid credentials");
        Response<ErrorResponse>(500, "Internal server error");
    }
}
