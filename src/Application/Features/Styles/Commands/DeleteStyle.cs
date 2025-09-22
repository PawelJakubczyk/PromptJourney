using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Application.Features.Common.Responses;
using Application.Extension;

namespace Application.Features.Styles.Commands.RemoveStyle;

public static class DeleteStyle
{
    public sealed record Command(string StyleName) : ICommand<DeleteResponse>;

    public sealed class Handler(
        IStyleRepository styleRepository,
        IExampleLinksRepository exampleLinksRepository
    ) : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(styleName)
                .IfStyleNotExists(styleName.Value, _styleRepository, cancellationToken)
                .ExecuteAndMapResultIfNoErrors(
                    async () =>
                    {
                        await _exampleLinksRepository.DeleteAllExampleLinksByStyleAsync(styleName.Value, cancellationToken);
                        return await _styleRepository.DeleteStyleAsync(styleName.Value, cancellationToken);
                    },
                    _ => DeleteResponse.Success($"Style '{styleName.Value.Value}' was successfully deleted.")
                );

            return result;
        }

    }
}
