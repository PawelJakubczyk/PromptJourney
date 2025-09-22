using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Application.Extension;

namespace Application.Features.Styles.Commands.RemoveTagInStyle;

public static class DeleteTagFromStyle
{
    public sealed record Command(string StyleName, string Tag) : ICommand<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);
            var tag = Tag.Create(command.Tag);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(styleName)
                .CollectErrors(tag)
                .IfStyleNotExists(styleName.Value, _styleRepository, cancellationToken)
                .IfTagNotExist(styleName.Value, tag.Value, _styleRepository, cancellationToken)
                .ExecuteAndMapResultIfNoErrors(
                    () => _styleRepository.DeleteTagFromStyleAsync(styleName.Value, tag.Value, cancellationToken),
                    StyleResponse.FromDomain
                );

            return result;
        }
    }
}