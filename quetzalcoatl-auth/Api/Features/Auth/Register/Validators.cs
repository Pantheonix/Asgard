namespace Api.Features.Auth.Register;

public class Validator : Validator<RegisterUserRequest>
{
    public Validator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(ApplicationUserConsts.UsernameMinLength)
            .WithMessage(
                $"Username must be at least {ApplicationUserConsts.UsernameMinLength} characters long"
            );

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is invalid");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(ApplicationUserConsts.PasswordMinLength)
            .WithMessage(
                $"Password must be at least {ApplicationUserConsts.PasswordMinLength} characters long"
            )
            .MaximumLength(ApplicationUserConsts.PasswordMaxLength)
            .WithMessage(
                $"Password must be at most {ApplicationUserConsts.PasswordMaxLength} characters long"
            )
            .Matches(ApplicationUserConsts.PasswordRegex)
            .WithMessage(
                "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character"
            );

        RuleFor(x => x.Fullname)
            .MaximumLength(ApplicationUserConsts.FullnameMaxLength)
            .WithMessage(
                $"Fullname must be at most {ApplicationUserConsts.FullnameMaxLength} characters long"
            )
            .When(x => !string.IsNullOrWhiteSpace(x.Fullname));

        RuleFor(x => x.Bio)
            .MaximumLength(ApplicationUserConsts.BioMaxLength)
            .WithMessage(
                $"Bio must be at most {ApplicationUserConsts.BioMaxLength} characters long"
            )
            .When(x => !string.IsNullOrWhiteSpace(x.Bio));

        RuleFor(x => x.ProfilePicture)
            .Cascade(CascadeMode.Stop)
            .Must(x => IsAllowedSize(x!.Length))
            .WithMessage("Profile picture size is invalid")
            .Must(x => IsAllowedType(x!.ContentType))
            .WithMessage("Profile picture must be a valid image")
            .When(x => x.ProfilePicture is not null);
    }

    private static bool IsAllowedSize(long length) =>
        length <= ApplicationUserConsts.ProfilePictureMaxLength;

    private static bool IsAllowedType(string contentType) =>
        ApplicationUserConsts.AllowedProfilePictureTypes.Contains(contentType);
}
