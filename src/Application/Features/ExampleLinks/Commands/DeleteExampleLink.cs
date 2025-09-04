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

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfLinkNotExists(link.Value, _exampleLinkRepository);

            var validationErrors = CreateValidationErrorIfAny<DeleteResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var deleteResult = await _exampleLinkRepository.DeleteExampleLinkAsync(link.Value);

            if (deleteResult.IsFailed) 
                return Result.Fail<DeleteResponse>(deleteResult.Errors);

            var response = DeleteResponse.Success($"Example link '{link.Value.Value}' was successfully deleted.");

            return Result.Ok(response);
        }
    }
}