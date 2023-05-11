namespace Api.Features.Auth.RefreshToken;

public class UserTokenResponseToRefreshTokenEntityProfile : Profile
{
    public UserTokenResponseToRefreshTokenEntityProfile()
    {
        CreateMap<UserTokenResponse, Domain.Entities.RefreshToken>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.RefreshToken))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.RefreshExpiry));
    }
}
