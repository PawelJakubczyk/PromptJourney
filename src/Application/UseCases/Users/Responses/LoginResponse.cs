namespace Application.UseCases.Users.Responses;

public sealed record LoginResponse(string AccessToken, UserResponse User)
{
    public static LoginResponse FromDomain(string accessToken, UserResponse user)
        => new(accessToken, user);
}
