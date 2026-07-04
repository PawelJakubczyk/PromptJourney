using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.Entities;

public sealed class MidjourneyUser : IEntity
{
    // Columns
    public UserID UserId { get; private set; }
    public UserName UserName { get; private set; }
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; } = null!;
    public Role Role { get; private set; } = null!;
    public CreatedOn CreatedOn { get; private set; } = null!;
    public bool IsDeleted { get; private set; } = false;
    public DeletedAt DeletedAt { get; private set; } = DeletedAt.None;

    // Navigation properties
    private List<MidjourneyPromptHistory> Histories { get; set; } = [];
    public IReadOnlyCollection<MidjourneyPromptHistory> MidjourneyHistories => Histories;

    #pragma warning disable CS8618
    private MidjourneyUser() { } // parameterless constructor for EF Core
    #pragma warning restore CS8618

    private MidjourneyUser(
        UserID userId,
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
        Result<UserID> userIdResult,
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
        Result<UserID> userIdResult,
        Result<UserName> userNameResult,
        Result<Email> emailResult,
        Result<PasswordHash> passwordHashResult,
        Result<Role> roleResult,
        Result<CreatedOn>? createdOnResult = null
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
                    Role = roleResult.Value,
                    CreatedOn = createdOnResult?.Value ?? CreatedOn.Create().Value
                };

                return Result.Ok(user);
            })
            .MapResult<MidjourneyUser>();

        return result;
    }

    // Update methods
    public Result<MidjourneyUser> SoftDelete()
    {
        var deletedAtResult = DeletedAt.Create();

        if (deletedAtResult.IsFailed)
            return Result.Fail<MidjourneyUser>(deletedAtResult.Errors);

        DeletedAt = deletedAtResult.Value;
        IsDeleted = true;

        return Result.Ok(this);
    }

    public Result<MidjourneyUser> Restore()
    {
        var deletedAtResult = DeletedAt.None;

        DeletedAt = deletedAtResult;
        IsDeleted = false;

        return Result.Ok(this);
    }

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
