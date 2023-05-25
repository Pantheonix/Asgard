namespace Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task CreateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct);
    Task DeleteRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct);
    Task<RefreshToken?> GetRefreshTokenAsync(
        Expression<Func<RefreshToken, bool>>? filter = null,
        Func<IQueryable<RefreshToken>, IOrderedQueryable<RefreshToken>>? orderBy = null);
}
