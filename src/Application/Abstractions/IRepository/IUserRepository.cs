using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;

namespace Application.Abstractions.IRepository;

public interface IUserRepository
{
    Task<Result<MidjourneyUser>> GetUserByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<Result<MidjourneyUser>> GetUserByNameAsync(UserName userName, CancellationToken cancellationToken);
    Task<Result<MidjourneyUser>> GetUserByIdAsync(UserID id, CancellationToken cancellationToken);
    Task<Result<bool>> CheckUserNameExistsAsync(UserName userName, CancellationToken cancellationToken);
    Task<Result<bool>> CheckUserEmailExistsAsync(Email email, CancellationToken cancellationToken);
    Task<Result<bool>> CheckUserIdExistsAsync(UserID id, CancellationToken cancellationToken);
    Task<Result<MidjourneyUser>> RegisterUserAsync(MidjourneyUser user, CancellationToken cancellationToken);
    Task<Result<MidjourneyUser>> DeleteUserByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<Result<MidjourneyUser>> DeleteUserByIdAsync(UserID id, CancellationToken cancellationToken);
    Task<Result<MidjourneyUser>> SoftDeleteUserByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<Result<MidjourneyUser>> SoftDeleteUserByIdAsync(UserID id, CancellationToken cancellationToken);
    Task<Result<MidjourneyUser>> UpdateUserEmailByIdAsync(UserID id, Email newEmail, CancellationToken cancellationToken);
    Task<Result<MidjourneyUser>> UpdateUserHashedPasswordAsync(Email email, PasswordHash newHashedPassword, CancellationToken cancellationToken);
    Task<Result<MidjourneyUser>> UpdateUserRoleByIdAsync(UserID id, Role newRole, CancellationToken cancellationToken);
    Task<Result<bool>> CheckUserExistsByEmailOrUserNameAsync(Email email, UserName userName, CancellationToken cancellationToken);
}