using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Commands.AddStyle;

public static class AddStyle
{
    public sealed record Command
    (
        string Name,
        string Type,
        string? Description = null,
        List<string>? Tags = null
    ) : ICommand<string>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, string>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<string>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.Name);
            var styleType = StyleType.Create(command.Type);
            var description = command.Description is not null ? Description.Create(command.Description) : null;
            var tags = command.Tags?.Select(Tag.Create).ToList();

            var style = MidjourneyStyle.Create
            (
                styleName.Value,
                styleType.Value,
                description?.Value,
                tags
            );

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(style)
                .IfStyleAlreadyExists(styleName.Value, _styleRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _styleRepository
                    .AddStyleAsync(style.Value, cancellationToken))
                .MapResult(() => command.Name);

            return result;
        }
    }
}
