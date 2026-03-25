using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Commands;

public static class UpdateDescriptionInStyle
{
    public sealed record Command(string StyleName, string? NewDescription) : ICommand<string?>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, string?>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<string?>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);
            var description = command.NewDescription is not null ? Description.Create(command.NewDescription) : Result.Ok(Description.None);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CongregateErrors(
                    pipeline => pipeline.CollectErrors(styleName),
                    pipeline => pipeline.CollectErrors(description))
                .IfStyleNotExists(styleName, _styleRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _styleRepository
                    .UpdateStyleDescriptionAsync(styleName.Value, description.Value, cancellationToken))
                .MapResult(() => command.NewDescription);

            return result;
        }
    }
}
