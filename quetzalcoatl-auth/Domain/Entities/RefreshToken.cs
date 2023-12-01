namespace Domain.Entities;

[Table("RefreshTokens")]
[PrimaryKey(nameof(Token))]
public class RefreshToken
{
    public Guid Token { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsInvalidated { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = default!;
}
