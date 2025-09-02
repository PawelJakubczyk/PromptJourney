using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;

namespace Application.Features.Styles.Commands;

public static class UpdateStyle
{
    public sealed record Command
    (
        StyleName StyleName,
        StyleType Type,
        Description? Description = null,
        List<Tag>? Tags = null
    ) : ICommand<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(command.StyleName, _styleRepository);

            var styleResult = MidjourneyStyle.Create
            (
                command.StyleName,
                command.Type,
                command.Description,
                command.Tags
            );

            var domainErrors = styleResult.Errors;

            if (applicationErrors.Count != 0 || domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Application Errors", applicationErrors)
                    .WithMetadata("Domain Errors", domainErrors);
                return Result.Fail<MidjourneyStyle>(error);
            }

            return await _styleRepository.UpdateStyleAsync(styleResult.Value);
        }
    }
}