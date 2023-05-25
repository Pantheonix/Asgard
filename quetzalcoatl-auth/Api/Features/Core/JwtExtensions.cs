namespace Api.Features.Core;

public static class JwtExtensions
{
    public static ClaimsPrincipal? ExtractValidatedClaimsPrincipal(
        this string token,
        TokenValidationParameters validationParameters
    )
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out var validatedToken
            );
            return !IsJwtWithValidSecurityAlgorithm(validatedToken) ? null : claimsPrincipal;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return (validatedToken is JwtSecurityToken jwtSecurityToken)
            && jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
            );
    }
}
