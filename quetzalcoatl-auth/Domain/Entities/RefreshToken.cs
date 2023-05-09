namespace Domain.Entities;

[Table("RefreshTokens")]
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    public string Token { get; set; } = default!;
    public DateTime ExpiryDate { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = default!;
}
