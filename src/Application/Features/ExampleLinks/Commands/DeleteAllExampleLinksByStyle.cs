using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.Common.Responses;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.ExampleLinks.Commands;

public static class DeleteAllExampleLinksByStyle
{
    public sealed record Command(string StyleNameRaw) : ICommand<BulkDeleteResponse>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository
    ) : ICommandHandler<Command, BulkDeleteResponse>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<BulkDeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleNameRaw);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(styleName)
                .IfStyleNotExists(styleName.Value, _styleRepository, cancellationToken, true)
                .ExecuteAndMapResultIfNoErrors(
                    () => _exampleLinkRepository.DeleteAllExampleLinksByStyleAsync(styleName.Value, cancellationToken),
                    count => BulkDeleteResponse.Success(
                        count,
                        $"Successfully deleted example links for style '{styleName.Value}'."
                    )
                );

            return result;
        }
    }
}
