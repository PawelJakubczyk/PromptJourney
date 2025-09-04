using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Queries;

public static class GetStylesByTags
{
    public sealed record Query(List<string>? Tags) : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .IfListIsEmpty<string>(query.Tags);

            var tags = query.Tags?.Select(t => Tag.Create(t)).ToList();

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    domainErrors
                        .CollectErrors<Tag>(tag);
                }
            }

            var validationErrors = CreateValidationErrorIfAny<List<StyleResponse>>(domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _styleRepository.GetStylesByTagsAsync(tags?.Select(t => t.Value).ToList() ?? []);

            if (result.IsFailed)
                return Result.Fail<List<StyleResponse>>(result.Errors);

            var responses = result.Value
                .Select(StyleResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}