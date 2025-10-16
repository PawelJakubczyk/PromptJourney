using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Common.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Commands;

public static class DeleteExampleLink
{
    public sealed record Command(string Id) : ICommand<DeleteResponse>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var linkId = MidjourneyStyleExampleLink.ParseLinkId(command.Id);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(linkId)
                .IfLinkNotExists(linkId.Value, _exampleLinkRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _exampleLinkRepository
                    .DeleteExampleLinkAsync(linkId.Value, cancellationToken))
                .MapResult(() => DeleteResponse.Success($"Example link with ID '{linkId}' was successfully deleted."));

            return result;
        }
    }
}
