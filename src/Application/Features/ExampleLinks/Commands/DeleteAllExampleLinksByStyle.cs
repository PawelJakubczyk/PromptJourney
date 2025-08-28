using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.ExampleLinks.Responses;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.ExampleLinks.Commands;

public static class DeleteAllExampleLinksByStyle
{
    public sealed record Command(StyleName Style) : ICommand<List<MidjourneyStyleExampleLink>>;

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

            return await _exampleLinkRepository.DeleteAllExampleLinkByStyleAsync(command.Style);
        }
    }
}