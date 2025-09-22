using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.Common.Responses;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.ExampleLinks.Commands;

public static class DeleteExampleLink
{
    public sealed record Command(string Link) : ICommand<DeleteResponse>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var link = ExampleLink.Create(command.Link);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors<ExampleLink>(link)
                .IfLinkNotExists(link.Value, _exampleLinkRepository, cancellationToken)
                .ExecuteAndMapResultIfNoErrors(
                    () => _exampleLinkRepository.DeleteExampleLinkAsync(link.Value, cancellationToken),
                    _ => DeleteResponse.Success($"Example link '{link.Value.Value}' was successfully deleted.")
                );

            return result;
        }

    }
}
