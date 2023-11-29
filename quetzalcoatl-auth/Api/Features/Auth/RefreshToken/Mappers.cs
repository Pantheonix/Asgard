namespace Api.Features.Auth.RefreshToken;

public class UserTokenResponseToRefreshTokenEntityProfile : Profile
{
    public UserTokenResponseToRefreshTokenEntityProfile()
    {
        CreateMap<UserTokenResponse, Domain.Entities.RefreshToken>()
            .ForMember(dest => dest.Jti, opt => opt.MapFrom<JtiResolver>())
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.RefreshToken))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.RefreshExpiry))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}

public class JtiResolver : IValueResolver<UserTokenResponse, Domain.Entities.RefreshToken, Guid>
{
    private readonly TokenValidationParameters _tokenValidationParameters;

    public JtiResolver(TokenValidationParameters tokenValidationParameters)
    {
        _tokenValidationParameters =
            tokenValidationParameters
            ?? throw new ArgumentNullException(nameof(tokenValidationParameters));
    }

    public Guid Resolve(
        UserTokenResponse source,
        Domain.Entities.RefreshToken destination,
        Guid destMember,
        ResolutionContext context
    )
    {
        return Guid.Parse(
            source
                .AccessToken
                .ExtractValidatedClaimsPrincipal(_tokenValidationParameters)!
                .Claims
                .Single(x => x.Type == JwtRegisteredClaimNames.Jti)
                .Value
        );
    }
}
