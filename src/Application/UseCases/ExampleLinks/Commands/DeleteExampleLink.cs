using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Common.Responses;
using Domain.ValueObjects;
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
            if (!Guid.TryParse(command.Id, out var linkId)) {
                return Result.Fail("Invalid ID format");
            }

            var result = await WorkflowPipeline
                .EmptyAsync()
                .IfLinkNotExists(linkId, _exampleLinkRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _exampleLinkRepository
                    .DeleteExampleLinkAsync(linkId, cancellationToken))
                .MapResult(() => DeleteResponse.Success($"Example link with ID '{linkId}' was successfully deleted."));

            return result;
        }
    }
}
