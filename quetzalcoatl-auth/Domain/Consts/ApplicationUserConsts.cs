namespace Domain.Consts;

public static class ApplicationUserConsts
{
    public const int UsernameMinLength = 3;
    public const int PasswordMinLength = 6;
    public const int PasswordMaxLength = 20;
    public const int FullnameMaxLength = 50;
    public const int BioMaxLength = 300;
    public const int ProfilePictureMaxLength = 10_000_000;
    public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,20}$";
    public static readonly string[] AllowedProfilePictureTypes =
    {
        "image/png",
        "image/jpeg",
        "image/jpg"
    };
}
