namespace Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task CreateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct)
    {
        _context.RefreshTokens.Remove(refreshToken);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(Expression<Func<RefreshToken, bool>>? filter,
        Func<IQueryable<RefreshToken>, IOrderedQueryable<RefreshToken>>? orderBy)
    {
        IQueryable<RefreshToken> query = _context.RefreshTokens;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (orderBy is not null)
        {
            return await orderBy(query).FirstOrDefaultAsync();
        }
       
        return await query.FirstOrDefaultAsync();
    }
}
