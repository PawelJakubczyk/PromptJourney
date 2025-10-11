using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Commands.AddTagToStyle;

public static class AddTagToStyle
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
                .Validate(pipeline => pipeline
                    .CollectErrors(styleName)
                    .CollectErrors(tag))
                .Validate(pipeline => pipeline
                    .IfStyleNotExists(styleName.Value, _styleRepository, cancellationToken)
                    .IfTagAlreadyExists(styleName.Value, tag.Value, _styleRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _styleRepository
                    .AddTagToStyleAsync(styleName.Value, tag.Value, cancellationToken))
                .MapResult<MidjourneyStyle, StyleResponse>
                    (style => StyleResponse.FromDomain(style));

            return result;
        }
    }
}