using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.Styles.Commands.AddStyle;

public static class AddStyle
{
    public sealed record Command
    (
        string Name,
        string Type,
        string? Description = null,
        List<string>? Tags = null
    ) : ICommand<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Command command, CancellationToken cancellationToken)
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

            var result = await ValidationPipeline
                .EmptyAsync()
                .CollectErrors(style)
                .IfStyleAlreadyExists(styleName.Value, _styleRepository, cancellationToken)
                .IfNoErrors()
                    .Executes(() => _styleRepository.AddStyleAsync(style.Value, cancellationToken))
                        .MapResult(StyleResponse.FromDomain);


            return result;
        }

    }
}