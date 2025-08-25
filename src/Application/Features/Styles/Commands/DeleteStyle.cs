using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.Styles.Commands.RemoveStyle;

public static class DeleteStyle
{
    public sealed record Command(string StyleName) : ICommand<MidjourneyStyle>;

    public sealed class Handler(
        IStyleRepository styleRepository,
        IExampleLinksRepository exampleLinksRepository
    ) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Style.MustExists(command.StyleName, _styleRepository);
            await _exampleLinksRepository.DeleteAllExampleLinkByStyleAsync(command.StyleName);

            return await _styleRepository.DeleteStyleAsync(command.StyleName);
        }
    }
}
