using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Common.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Commands;

public static class DeleteExampleLinkByName
{
    public sealed record Command(string? Name) : ICommand<DeleteResponse>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var exampleLink = ExampleLink.Create(command.Name);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(exampleLink)
                .IfLinkNotExists(exampleLink.Value, _exampleLinkRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _exampleLinkRepository
                    .DeleteExampleLinkByNameAsync(exampleLink.Value, cancellationToken))
                .MapResult(() => DeleteResponse.Success($"Example link with name '{exampleLink}' was successfully deleted."));

            return result;
        }
    }
}
