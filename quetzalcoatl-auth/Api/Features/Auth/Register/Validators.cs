namespace Api.Features.Auth.Register;

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

        RuleFor(x => x.Fullname)
            .MaximumLength(50)
            .WithMessage("Fullname must be at most 50 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.Fullname));

        RuleFor(x => x.Bio)
            .MaximumLength(300)
            .WithMessage("Bio must be at most 300 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.Bio));

        RuleFor(x => x.ProfilePicture)
            .Cascade(CascadeMode.Stop)
            .Must(x => IsAllowedSize(x!.Length))
            .WithMessage("Profile picture size is invalid")
            .Must(x => IsAllowedType(x!.ContentType))
            .WithMessage("Profile picture must be a valid image")
            .When(x => x.ProfilePicture is not null);
    }

    private static bool IsAllowedSize(long length) => length <= 10_000_000;

    private static bool IsAllowedType(string contentType) =>
        contentType.ToLower() is "image/png" or "image/jpeg" or "image/jpg";
}
