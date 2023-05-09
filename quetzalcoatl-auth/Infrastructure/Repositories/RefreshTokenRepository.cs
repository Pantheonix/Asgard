namespace Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task StoreRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct)
    {
        _context.RefreshTokens.Add(refreshToken);
        return _context.SaveChangesAsync(ct);
    }

    public async Task<bool> IsTokenValidAsync(string userId, string token, CancellationToken ct)
    {
        var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(
            rt => rt.UserId == Guid.Parse(userId) && rt.Token == token && rt.ExpiryDate >= DateTime.Now,
            ct
        );
        return refreshToken is not null;
    }
}
