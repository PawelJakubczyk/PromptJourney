using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Commands;

public static class AddStyle
{
    public sealed record Command
    (
        string Name,
        string Type,
        string? Description = null,
        List<string?>? Tags = null
    ) : ICommand<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.Name);
            var styleType = StyleType.Create(command.Type);
            var description = command.Description is not null ? Description.Create(command.Description) : Result.Ok(Description.None);
            var tags = command.Tags is not null ? TagsCollection.Create(command.Tags) : Result.Ok(TagsCollection.None);

            var style = MidjourneyStyle.Create
            (
                styleName,
                styleType,
                description,
                tags
            );

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(style)
                .IfStyleAlreadyExists(styleName.Value, _styleRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _styleRepository
                    .AddStyleAsync(style.Value, cancellationToken))
                .MapResult(() => StyleResponse.FromDomain(style.Value));

            return result;
        }
    }
}
