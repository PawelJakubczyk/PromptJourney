using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record ParamsCollection : ValueObject<List<Param>>, ICreatable<ParamsCollection, List<string>?>
{
    private ParamsCollection(List<Param> value) : base(value) { }

    public static readonly ParamsCollection None = new([]);
    public override bool IsNone => this == None;

    public static Result<ParamsCollection> Create(List<string>? @params)
    {

        if (@params is null || @params.Count == 0)
        {
            return Result.Ok(None);
        }

        var paramsCollection = new List<Result<Param>>();

        foreach (string param in @params?.Select(p => p.Trim().ToLower()).Distinct() ?? [])
        {
            if (!string.IsNullOrWhiteSpace(param))
            {
                paramsCollection.Add(Param.Create(param));
            }
        }


        var result = WorkflowPipeline
        .Empty()
        .CollectErrorsFromList(paramsCollection)
        .ExecuteIfNoErrors<ParamsCollection>(() => new ParamsCollection([.. paramsCollection!.Select(param => param.Value)]))
        .MapResult<ParamsCollection>();

        return result;
    }
}