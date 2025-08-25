using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.Styles.Commands.AddStyle;

public static class UpdateStyle
{
    public sealed record Command
    (
        string Name,
        string Type,
        string? Description = null,
        ICollection<string>? Tags = null
    ) : ICommand<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Style.MustExists(command.Name, _styleRepository);

            var style = MidjourneyStyle.Create(
                command.Name,
                command.Type,
                command.Description,
                command.Tags
            );

            if (style.IsFailed)
                return Result.Fail<MidjourneyStyle>(style.Errors);

            return await _styleRepository.UpdateStyleAsync(style.Value);
        }
    }
}