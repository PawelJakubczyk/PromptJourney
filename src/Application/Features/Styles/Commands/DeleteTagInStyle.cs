using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.Styles.Commands.RemoveTagInStyle;

public static class DeleteTagInStyle
{
    public sealed record Command(string StyleName, string Tag) : ICommand<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Style.MustExists(command.StyleName, _styleRepository);
            await Validate.Tag.MustExists(command.StyleName, command.Tag, _styleRepository);

            return await _styleRepository.DeleteTagFromStyleAsync(command.StyleName, command.Tag);
        }
    }
}