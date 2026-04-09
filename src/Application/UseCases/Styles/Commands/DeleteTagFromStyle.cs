using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Commands;

public static class DeleteTagFromStyle
{
    public sealed record Command(string StyleName, string Tag) : ICommand<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);
            var tag = Tag.Create(command.Tag);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CongregateErrors(
                    pipeline => pipeline.CollectErrors(styleName),
                    pipeline => pipeline.CollectErrors(tag))
                .CongregateErrors(
                    pipeline => pipeline.IfStyleNotExists(styleName, _styleRepository, cancellationToken),
                    pipeline => pipeline.IfTagNotExist(styleName, tag, _styleRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _styleRepository.DeleteTagFromStyleAsync(styleName.Value, tag.Value, cancellationToken))
                .MapResult<MidjourneyStyle, StyleResponse>
                    (style => StyleResponse.FromDomain(style));

            return result;
        }
    }
}