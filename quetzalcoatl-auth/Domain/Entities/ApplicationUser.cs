using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    [MaxLength(50)]
    public string? Fullname { get; set; }

    [MaxLength(300)]
    public string? Bio { get; set; }
}
