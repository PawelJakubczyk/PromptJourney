using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.Styles.Commands;

public static class UpdateStyle
{
    public sealed record Command
    (
        string StyleName,
        string Type,
        string? Description = null,
        List<string>? Tags = null
    ) : ICommand<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);
            var type = StyleType.Create(command.Type);
            var description = command.Description is not null ? Description.Create(command.Description) : null;
            var tags = command.Tags?.Select(Tag.Create).ToList();

            var midjourneyStyle = MidjourneyStyle.Create(styleName, type, description!, tags);


            var result = await ValidationPipeline
                .EmptyAsync()
                .CollectErrors(midjourneyStyle)
                .IfStyleNotExists(styleName.Value, _styleRepository, cancellationToken)
                .IfNoErrors()
                    .Executes(() => _styleRepository.UpdateStyleAsync(midjourneyStyle.Value, cancellationToken))
                        .MapResult(StyleResponse.FromDomain);


            return result;
        }
    }
}