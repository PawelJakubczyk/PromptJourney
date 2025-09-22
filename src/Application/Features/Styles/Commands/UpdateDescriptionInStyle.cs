using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Application.Extension;

namespace Application.Features.Styles.Commands;

public static class UpdateDescriptionInStyle
{
    public sealed record Command(string StyleName, string NewDescription) : ICommand<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);
            var description = Description.Create(command.NewDescription);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(styleName)
                .CollectErrors(description)
                .IfStyleNotExists(styleName.Value, _styleRepository, cancellationToken)
                .ExecuteAndMapResultIfNoErrors(
                    () => _styleRepository.UpdateStyleDescriptionAsync(styleName.Value, description.Value, cancellationToken),
                    StyleResponse.FromDomain
                );

            return result;
        }
    }
}