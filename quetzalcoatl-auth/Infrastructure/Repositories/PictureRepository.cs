namespace Infrastructure.Repositories;

public class PictureRepository : IPictureRepository
{
    private readonly AppDbContext _context;

    public PictureRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Picture?> GetPictureByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Pictures.FirstOrDefaultAsync(p => p.Id == id, cancellationToken: ct);
    }
}
