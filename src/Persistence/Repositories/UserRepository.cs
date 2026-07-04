using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Utilities.Errors;
using Utilities.Results;

namespace Persistence.Repositories;

public sealed class UserRepository(MidjourneyDbContext dbContext) : IUserRepository
{
    private readonly MidjourneyDbContext _dbContext = dbContext;

    // Query
    public async Task<Result<MidjourneyUser>> GetUserByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<MidjourneyUser>()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            return Result.Fail<MidjourneyUser>(ErrorFactories.NotFound(email));

        return Result.Ok(user);
    }

    public async Task<Result<MidjourneyUser>> GetUserByNameAsync(UserName userName, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<MidjourneyUser>()
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

        if (user is null)
            return Result.Fail<MidjourneyUser>(ErrorFactories.NotFound(userName));

        return Result.Ok(user);
    }

    public async Task<Result<MidjourneyUser>> GetUserByIdAsync(UserID id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<MidjourneyUser>()
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);

        if (user is null)
            return Result.Fail<MidjourneyUser>(ErrorFactories.NotFound(id));

        return Result.Ok(user);
    }

    public async Task<Result<bool>> CheckUserEmailExistsAsync(Email email, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Set<MidjourneyUser>()
            .AnyAsync(u => u.Email == email, cancellationToken);

        return Result.Ok(exists);
    }

    public async Task<Result<bool>> CheckUserNameExistsAsync(UserName userName, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Set<MidjourneyUser>()
            .AnyAsync(u => u.UserName == userName, cancellationToken);

        return Result.Ok(exists);
    }

    public async Task<Result<bool>> CheckUserIdExistsAsync(UserID id, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Set<MidjourneyUser>()
            .AnyAsync(u => u.UserId == id, cancellationToken);

        return Result.Ok(exists);
    }

    public async Task<Result<bool>> CheckUserExistsByEmailOrUserNameAsync(Email email, UserName userName, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Set<MidjourneyUser>()
            .AnyAsync(u => u.Email == email || u.UserName == userName, cancellationToken);

        return Result.Ok(exists);
    }

    // Commands
    public async Task<Result<MidjourneyUser>> RegisterUserAsync(MidjourneyUser user, CancellationToken cancellationToken)
    {
        await _dbContext.Set<MidjourneyUser>().AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(user);
    }

    public async Task<Result<MidjourneyUser>> DeleteUserByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<MidjourneyUser>()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            return Result.Fail<MidjourneyUser>(ErrorFactories.NotFound(email));

        _dbContext.Set<MidjourneyUser>().Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(user);
    }

    public async Task<Result<MidjourneyUser>> DeleteUserByIdAsync(UserID id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<MidjourneyUser>()
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);

        if (user is null)
            return Result.Fail<MidjourneyUser>(ErrorFactories.NotFound(id));

        _dbContext.Set<MidjourneyUser>().Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(user);
    }

    public async Task<Result<MidjourneyUser>> UpdateUserEmailByIdAsync(UserID id, Email newEmail, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<MidjourneyUser>()
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);

        if (user is null)
            return Result.Fail<MidjourneyUser>(ErrorFactories.NotFound(id));

        var updateResult = user.UpdateEmail(Result.Ok(newEmail));
        if (updateResult.IsFailed)
            return Result.Fail<MidjourneyUser>(updateResult.Errors);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(user);
    }

    public async Task<Result<MidjourneyUser>> UpdateUserHashedPasswordAsync(Email email, PasswordHash newHashedPassword, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<MidjourneyUser>()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            return Result.Fail<MidjourneyUser>(ErrorFactories.NotFound(email));

        var updateResult = user.UpdatePasswordHash(Result.Ok(newHashedPassword));
        if (updateResult.IsFailed)
            return Result.Fail<MidjourneyUser>(updateResult.Errors);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(user);
    }

    public async Task<Result<MidjourneyUser>> UpdateUserRoleByIdAsync(UserID id, Role newRole, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<MidjourneyUser>()
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);

        if (user is null)
            return Result.Fail<MidjourneyUser>(ErrorFactories.NotFound(id));

        var updateResult = user.UpdateRole(Result.Ok(newRole));
        if (updateResult.IsFailed)
            return Result.Fail<MidjourneyUser>(updateResult.Errors);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(user);
    }

    public async Task<Result<MidjourneyUser>> SoftDeleteUserByEmailAsync(
        Email email,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext
            .Set<MidjourneyUser>()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            return Result.Fail<MidjourneyUser>(ErrorFactories.NotFound(email));

        var softDeleteResult = user.SoftDelete();
        if (softDeleteResult.IsFailed)
            return Result.Fail<MidjourneyUser>(softDeleteResult.Errors);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(user);
    }

    public async Task<Result<MidjourneyUser>> SoftDeleteUserByIdAsync(
        UserID id,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext
            .Set<MidjourneyUser>()
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);

        if (user is null)
            return Result.Fail<MidjourneyUser>(ErrorFactories.NotFound(id));

        var softDeleteResult = user.SoftDelete();
        if (softDeleteResult.IsFailed)
            return Result.Fail<MidjourneyUser>(softDeleteResult.Errors);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(user);
    }
}