namespace Infrastructure.Repositories;

public class PictureRepository : IPictureRepository
{
    private readonly ApplicationDbContext _context;

    public PictureRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Picture?> GetPictureByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Pictures.FirstOrDefaultAsync(p => p.Id == id, cancellationToken: ct);
    }
}
