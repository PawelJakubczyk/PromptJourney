using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;

namespace Application.Features.Styles.Commands.RemoveTagInStyle;

public static class DeleteTagInStyle
{
    public sealed record Command(StyleName StyleName, Tag Tag) : ICommand<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(command.StyleName)
                .CollectErrors<Tag>(command.Tag);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(command.StyleName, _styleRepository)
                .IfTagNotExists(command.StyleName, command.Tag, _styleRepository);

            if (applicationErrors.Count != 0 || domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Application Errors", applicationErrors)
                    .WithMetadata("Domain Errors", domainErrors);
                return Result.Fail<MidjourneyStyle>(error);
            }

            return await _styleRepository.DeleteTagFromStyleAsync(command.StyleName, command.Tag);
        }
    }
}