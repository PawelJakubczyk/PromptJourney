using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.ExampleLinks.Commands;

public static class DeleteExampleLink
{
    public sealed record Command(string Link) : ICommand<MidjourneyStyleExampleLink>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : ICommandHandler<Command, MidjourneyStyleExampleLink>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;

        public async Task<Result<MidjourneyStyleExampleLink>> Handle(Command command, CancellationToken cancellationToken)
        {

            await Validate.Link.Input.MustNotBeNullOrEmpty(command.Link);
            await Validate.Link.Input.MustHaveMaximumLength(command.Link);
            await Validate.Link.ShouldExists(command.Link, _exampleLinkRepository);

            return await _exampleLinkRepository.DeleteExampleLinkAsync(command.Link);
        }
    }
}