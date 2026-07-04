using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Queries;

public static class CheckUserEmailExists
{
    public sealed record Query(string Email)
        : IQuery<bool>;

    public sealed class Handler(IUserRepository userRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken ct)
        {
            var email = Email.Create(query.Email);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(email)
                .ExecuteIfNoErrors(() => _userRepository
                    .CheckUserEmailExistsAsync(email.Value, ct))
                .MapResult<bool>();

            return result;
        }
    }
}
