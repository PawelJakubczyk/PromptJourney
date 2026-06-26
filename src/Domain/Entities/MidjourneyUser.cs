using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.Entities;

public sealed class MidjourneyUser : IEntity
{
    // Columns
    public UserId UserId { get; private set; }
    public UserName UserName { get; private set; }
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; } = null!;
    public Role Role { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

#pragma warning disable CS8618
    private MidjourneyUser() { } // parameterless constructor for EF Core
    #pragma warning restore CS8618

    private MidjourneyUser(
        UserId userId,
        UserName userName,
        Email email
    )
    {
        UserId = userId;
        UserName = userName;
        Email = email;
    }

    // Basic creation (without password/role)
    public static Result<MidjourneyUser> Create(
        Result<UserId> userIdResult,
        Result<UserName> userNameResult,
        Result<Email> emailResult
    )
    {
        var result = WorkflowPipeline
            .Empty()
            .AggregateErrors(
                pipeline => pipeline.CollectErrors(userIdResult),
                pipeline => pipeline.CollectErrors(userNameResult),
                pipeline => pipeline.CollectErrors(emailResult)
            )
            .ExecuteIfNoErrors(() =>
            {
                var user = new MidjourneyUser(
                    userIdResult.Value,
                    userNameResult.Value,
                    emailResult.Value
                );

                return Result.Ok(user);
            })
            .MapResult<MidjourneyUser>();

        return result;
    }

    // Full registration (with password + role)
    public static Result<MidjourneyUser> Register(
        Result<UserId> userIdResult,
        Result<UserName> userNameResult,
        Result<Email> emailResult,
        Result<PasswordHash> passwordHashResult,
        Result<Role> roleResult
    )
    {
        var result = WorkflowPipeline
            .Empty()
            .AggregateErrors(
                pipeline => pipeline.CollectErrors(userIdResult),
                pipeline => pipeline.CollectErrors(userNameResult),
                pipeline => pipeline.CollectErrors(emailResult),
                pipeline => pipeline.CollectErrors(passwordHashResult),
                pipeline => pipeline.CollectErrors(roleResult)
            )
            .ExecuteIfNoErrors(() =>
            {
                var user = new MidjourneyUser(
                    userIdResult.Value,
                    userNameResult.Value,
                    emailResult.Value
                )
                {
                    PasswordHash = passwordHashResult.Value,
                    Role = roleResult.Value
                };

                return Result.Ok(user);
            })
            .MapResult<MidjourneyUser>();

        return result;
    }

    // Update methods
    public Result<UserName> UpdateUserName(Result<UserName> userNameResult) =>
        UpdateValue(userNameResult, name => UserName = name);

    public Result<Email> UpdateEmail(Result<Email> emailResult) =>
        UpdateValue(emailResult, email => Email = email);
    
    public Result<PasswordHash> UpdatePasswordHash(Result<PasswordHash> passwordHashResult) =>
        UpdateValue(passwordHashResult, passwordHash => PasswordHash = passwordHash);

    public Result<Role> UpdateRole(Result<Role> roleResult) =>
        UpdateValue(roleResult, role => Role = role);

    private Result<TValue> UpdateValue<TValue>(
        Result<TValue> valueResult,
        Action<TValue> setter)
    {
        if (valueResult.IsFailed)
            return Result.Fail<TValue>(valueResult.Errors);

        setter(valueResult.Value);

        return Result.Ok(valueResult.Value);
    }
}
