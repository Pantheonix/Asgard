namespace Api.Auth.Register;

public class RegisterUserRequest
{
    public string Username { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
}

public class Validator : Validator<RegisterUserRequest>
{
    public Validator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is invalid");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long")
            .MaximumLength(20)
            .WithMessage("Password must be at most 20 characters long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,20}$")
            .WithMessage(
                "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character"
            );
    }
}

public class RegisterUserResponse
{
    public string Username { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string Token { get; set; } = String.Empty;
}
