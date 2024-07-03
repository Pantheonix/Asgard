namespace Api.Features.Users.Update;

public class Validator : Validator<UpdateUserRequest>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Username)
            .MinimumLength(ApplicationUserConsts.UsernameMinLength)
            .WithMessage(
                $"Username must be at least {ApplicationUserConsts.UsernameMinLength} characters long"
            )
            .When(x => !x.Username.IsNullOrEmpty());

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email is invalid")
            .When(x => !x.Email.IsNullOrEmpty());

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
