using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record TagsCollection : ValueObject<List<Tag>>, IValueObjectCollection<string>, ICreatable<TagsCollection, List<string?>?>
{
    public const int MaxLength = 50;

    public static readonly TagsCollection None = new([]);
    public override bool IsNone => this == None;
    public List<string> CollectionValues => [.. Value.Select(t => t.Value)];
    private TagsCollection(List<Tag> value) : base(value) { }

    public static Result<TagsCollection> Create(List<string?>? tags)
    {
        var tagsCollection = new List<Result<Tag>>();

        foreach (string? tag in tags?.Select(tag => tag?.Trim().ToLower()).Distinct() ?? [])
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                tagsCollection.Add(Tag.Create(tag));
            }
        }

        if (tagsCollection is null || tagsCollection.Count == 0)
            return Result.Ok(None);

        var result = WorkflowPipeline
            .Empty()
            .CollectErrorsFromList(tagsCollection)
            .ExecuteIfNoErrors<TagsCollection>(() => new TagsCollection([.. tagsCollection!.Select(tag => tag.Value)]))
            .MapResult<TagsCollection>();

        return result;
    }

    public static Result<TagsCollection> AddTag(TagsCollection tagsCollection, Tag tag)
    {
        var result = WorkflowPipeline
            .Empty()
            .CollectErrors<Tag>(tag)
            .IfListContain<Tag>(tagsCollection.Value, tag)
            .ExecuteIfNoErrors<TagsCollection>(() =>
            {
                if (tagsCollection.IsNone)
                    return Create([tag.Value]).Value;
                return new TagsCollection([.. tagsCollection.Value, tag]);
            }   
            )
            .MapResult<TagsCollection>();

            return result;
    }

    public static Result<TagsCollection> RemoveTag(TagsCollection tagsCollection, Tag tag)
    {
        var result = WorkflowPipeline
            .Empty()
            .CollectErrors<Tag>(tag)
            .IfListNotContain<Tag>(tagsCollection.Value, tag)
            .ExecuteIfNoErrors<TagsCollection>(() =>
            {
                var updatedTags = tagsCollection.Value.Where(t => t != tag).ToList();
                if (updatedTags.Count == 0)
                    return None;

                return new TagsCollection(updatedTags);
            })
            .MapResult<TagsCollection>();
        return result;
    }
}

public static class ListValidationExtension
{
    public static WorkflowPipeline IfListNotContain<TValue>(
        this WorkflowPipeline pipeline,
        List<TValue>? items,
        TValue element)
        where TValue : ValueObject<string>
    {
        if (items != null && !items.Contains(element))
        {
            pipeline.Errors.Add(
                ErrorFactories.CollectionNotContain<TValue>(items)
            );
        }
        return pipeline;
    }

    public static WorkflowPipeline IfListContain<TValue>(
        this WorkflowPipeline pipeline,
        List<TValue>? items,
        TValue element)
        where TValue : ValueObject<string>
    {
        if (items != null && items.Contains(element))
        {
            pipeline.Errors.Add(
                ErrorFactories.CollectionAlreadyContains<TValue>(element)
            );
        }
        return pipeline;
    }
}