using Domain.Entities;

namespace Application.Abstractions.Auth;

public interface ITokenService
{
    string GenerateAccessToken(MidjourneyUser user);
}
