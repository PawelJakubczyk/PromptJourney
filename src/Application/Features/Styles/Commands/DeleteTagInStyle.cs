using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;

namespace Application.Features.Styles.Commands.RemoveTagInStyle;

public static class DeleteTagInStyle
{
    public sealed record Command(StyleName StyleName, Tag Tag) : ICommand<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(command.StyleName, _styleRepository)
                .IfTagNotExists(command.StyleName, command.Tag, _styleRepository);

            return await _styleRepository.DeleteTagFromStyleAsync(command.StyleName, command.Tag);
        }
    }
}