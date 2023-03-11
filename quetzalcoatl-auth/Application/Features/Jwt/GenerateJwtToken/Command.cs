namespace Application.Features.Jwt.GenerateJwtToken;

public class GenerateJwtTokenCommand : ICommand<string>
{
    public ApplicationUser User { get; set; } = default!;
}
