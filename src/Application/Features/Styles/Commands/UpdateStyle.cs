using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.Styles.Commands;

public static class UpdateStyle
{
    public sealed record Command
    (
        string StyleName,
        string Type,
        string? Description = null,
        List<string>? Tags = null
    ) : ICommand<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);
            var type = StyleType.Create(command.Type);
            var description = command.Description != null ? Description.Create(command.Description) : null;
            var tags = command.Tags?.Select(t => Tag.Create(t)).ToList();

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(styleName.Value, _styleRepository);

            var styleResult = MidjourneyStyle.Create
            (
                styleName,
                type,
                description,
                tags
            );

            var domainErrors = styleResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<StyleResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _styleRepository.UpdateStyleAsync(styleResult.Value);

            if (result.IsFailed)
                return Result.Fail<StyleResponse>(result.Errors);

            var response = StyleResponse.FromDomain(result.Value);

            return Result.Ok(response);
        }
    }
}