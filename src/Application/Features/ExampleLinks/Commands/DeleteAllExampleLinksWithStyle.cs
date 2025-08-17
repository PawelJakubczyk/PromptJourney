using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.ExampleLinks.Commands;

public static class DeleteAllExampleLinksWithStyle
{
    public sealed record Command(string Style) : ICommand<List<MidjourneyStyleExampleLink>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository
    ) : ICommandHandler<Command, List<MidjourneyStyleExampleLink>>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<MidjourneyStyleExampleLink>>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Style.ShouldExists(command.Style, _styleRepository);
            //await Validate.ExampleLinks.ShouldHaveAtLastOneElementByVersion(command.Style, _styleRepository);

            return await _exampleLinkRepository.DeleteAllExampleLinkWithStyleAsync(command.Style);
        }
    }
}