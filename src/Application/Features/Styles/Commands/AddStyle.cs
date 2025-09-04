using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Styles.Responses;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.Styles.Commands.AddStyle;

public static class AddStyle
{
    public sealed record Command
    (
        string Name,
        string Type,
        string? Description = null,
        List<string>? Tags = null
    ) : ICommand<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var name = StyleName.Create(command.Name);
            var type = StyleType.Create(command.Type);
            var description = command.Description != null ? Description.Create(command.Description) : null;
            var tags = command.Tags?.Select(t => Tag.Create(t)).ToList();

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleAlreadyExists(name.Value, _styleRepository);

            var styleResult = MidjourneyStyle.Create(
                name.Value,
                type.Value,
                description?.Value,
                tags?.Select(t => t.Value).ToList()
            );

            var domainErrors = styleResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<StyleResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _styleRepository.AddStyleAsync(styleResult.Value);

            if (result.IsFailed)
                return Result.Fail<StyleResponse>(result.Errors);

            var response = StyleResponse.FromDomain(result.Value);

            return Result.Ok(response);
        }
    }
}