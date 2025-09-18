using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Common.Responses;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.ExampleLinks.Commands;

public static class DeleteExampleLink
{
    public sealed record Command(string Link) : ICommand<DeleteResponse>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var link = ExampleLink.Create(command.Link);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ExampleLink>(link);

            var deleteLinkResult = await _exampleLinkRepository.DeleteExampleLinkAsync(link.Value);

            var persitanceErrors = deleteLinkResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<DeleteResponse>
            (
                (nameof(domainErrors), domainErrors),
                (nameof(persitanceErrors), persitanceErrors)
            );

            if (validationErrors is not null) return validationErrors;

            var response = DeleteResponse.Success($"Example link '{link.Value.Value}' was successfully deleted.");

            return Result.Ok(response);
        }
    }
}