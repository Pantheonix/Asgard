namespace Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task CreateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct);
    Task DeleteRefreshTokenAsync(Expression<Func<RefreshToken, bool>>? filter = null);
    Task<RefreshToken?> GetRefreshTokenAsync(
        Expression<Func<RefreshToken, bool>>? filter = null,
        Func<IQueryable<RefreshToken>, IOrderedQueryable<RefreshToken>>? orderBy = null
    );
}
