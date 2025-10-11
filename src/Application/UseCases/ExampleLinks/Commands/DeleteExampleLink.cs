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
    public sealed record Command(string Link) : ICommand<DeleteResponse>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var link = ExampleLink.Create(command.Link);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors<ExampleLink>(link)
                .IfLinkNotExists(link.Value, _exampleLinkRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _exampleLinkRepository
                    .DeleteExampleLinkAsync(link.Value, cancellationToken))
                .MapResult(() => DeleteResponse.Success($"Example link '{link.Value.Value}' was successfully deleted."));

            return result;
        }
    }
}