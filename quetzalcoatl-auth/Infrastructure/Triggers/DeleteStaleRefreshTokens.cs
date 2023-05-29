namespace Infrastructure.Triggers;

public class DeleteStaleRefreshTokens : IBeforeSaveTrigger<RefreshToken>
{
    private readonly IRefreshTokenRepository _tokenRepository;
    
    public DeleteStaleRefreshTokens(IRefreshTokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
    }
    
    public async Task BeforeSave(ITriggerContext<RefreshToken> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType is ChangeType.Added or ChangeType.Modified)
        {
            await _tokenRepository.DeleteRefreshTokenAsync(
                token => token.IsInvalidated || token.IsUsed || token.ExpiryDate < DateTime.UtcNow 
            ); 
        }
    }
}