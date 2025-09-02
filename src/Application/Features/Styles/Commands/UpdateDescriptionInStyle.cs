using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;

namespace Application.Features.Styles.Commands;

public static class UpdateDescriptionInStyle
{
    public sealed record Command(StyleName StyleName, Description NewDescription) : ICommand<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(command.StyleName)
                .CollectErrors<Description>(command.NewDescription);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(command.StyleName, _styleRepository);

            if (applicationErrors.Count != 0 || domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Application Errors", applicationErrors)
                    .WithMetadata("Domain Errors", domainErrors);
                return Result.Fail<MidjourneyStyle>(error);
            }

            return await _styleRepository.UpadteStyleDescription(command.StyleName, command.NewDescription);
        }
    }
}