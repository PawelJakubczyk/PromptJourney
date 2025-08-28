using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.Styles.Commands.AddTagToStyle;

public static class AddTagToStyle
{
    public sealed record Command(StyleName StyleName, string Tag) : ICommand<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Style.ShouldExists(command.StyleName, _styleRepository);
            await Validate.Tags.ShouldNotExists(command.StyleName, command.Tag, _styleRepository);

            return await _styleRepository.AddTagToStyleAsync(command.StyleName, command.Tag);
        }
    }
}