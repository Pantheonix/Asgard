namespace Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task StoreRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct);
    Task<bool> IsTokenValidAsync(string userId, string token, CancellationToken ct);
}
