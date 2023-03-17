namespace Api.Features.Users.Update;

public class Validator : Validator<UpdateUserRequest>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Username)
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long")
            .When(x => !x.Username.IsNullOrEmpty());

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email is invalid")
            .When(x => !x.Email.IsNullOrEmpty());
    }
}
