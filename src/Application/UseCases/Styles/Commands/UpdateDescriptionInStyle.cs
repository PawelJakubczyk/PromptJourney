using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Commands;

public static class UpdateDescriptionInStyle
{
    public sealed record Command(string StyleName, string NewDescription) : ICommand<string>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, string>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<string>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);
            var description = Description.Create(command.NewDescription);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .Validate(pipeline => pipeline
                    .CollectErrors(styleName)
                    .CollectErrors(description))
                .IfStyleNotExists(styleName.Value, _styleRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _styleRepository
                    .UpdateStyleDescriptionAsync(styleName.Value, description.Value, cancellationToken))
                .MapResult(() => command.NewDescription);

            return result;
        }
    }
}
