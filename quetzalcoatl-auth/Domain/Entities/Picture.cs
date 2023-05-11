namespace Domain.Entities;

[Table("Pictures")]
public class Picture
{
    [Key]
    public Guid Id { get; set; }
    public byte[] Data { get; set; } = default!;
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = default!;
}
