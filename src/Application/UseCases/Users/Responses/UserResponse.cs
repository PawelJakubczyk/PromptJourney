using Domain.Entities;

namespace Application.UseCases.Users.Responses;

public sealed record UserResponse
(
    string UserId,
    string UserName,
    string UserEmail
)
{
    public static UserResponse FromDomain(MidjourneyUser user) =>
        new(
            user.UserId.Value.ToString(),
            user.UserName.Value,
            user.Email.Value!
        );
}