namespace Domain.Interfaces;

public interface IPictureRepository
{
    Task<Picture?> GetPictureByIdAsync(Guid id, CancellationToken ct);
}
