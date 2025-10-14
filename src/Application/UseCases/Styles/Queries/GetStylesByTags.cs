using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Extensions;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Queries;

public static class GetStylesByTags
{
    public sealed record Query(List<string>? Tags) : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var tags = query.Tags?.Select(Tag.Create).ToList();

            var result = await WorkflowPipeline
                .EmptyAsync()
                .IfListIsNullOrEmpty(query.Tags)
                .CollectErrors(tags!)
                .ExecuteIfNoErrors(() => _styleRepository
                    .GetStylesByTagsAsync(tags?.Select(t => t.Value).ToList() ?? [], cancellationToken))
                .MapResult<List<MidjourneyStyle>, List<StyleResponse>>
                    (styleList => [.. styleList.Select(StyleResponse.FromDomain)]);

            return result;
        }
    }
}

internal static class CollectionValidationExtensions
{
    public static async Task<WorkflowPipeline> IfListIsNullOrEmpty<TValue>
    (
        this Task<WorkflowPipeline> pipelineTask,
        List<TValue>? items
    )
    {
        var pipeline = await pipelineTask;
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError)
            return pipeline;

        if (items is null || items.Count == 0)
        {
            var name = typeof(TValue).Name;
            errors.Add
            (
            ErrorFactory.Create()
                .WithLayer<ApplicationLayer>()
                .WithMessage($"List of '{name}' must not be empty.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
            );
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}
