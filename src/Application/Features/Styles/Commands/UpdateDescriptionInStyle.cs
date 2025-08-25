using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.Styles.Commands.EditDescriptionInStyle;

public static class UpdateDescriptionInStyle
{
    public sealed record Command(string StyleName, string NewDescription) : ICommand<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Style.MustExists(command.StyleName, _styleRepository);

            return await _styleRepository.UpadteStyleDescription(command.StyleName, command.NewDescription);
        }
    }
}