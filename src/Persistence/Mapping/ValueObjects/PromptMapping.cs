using Domain.ValueObjects;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class PromptMapping
{
    public class PromptConverter : ValueConverter<Prompt, string>
    {
        public PromptConverter()
        : base
        (
            mv => mv.Value,                             // save to database
            value => Prompt.Create(value).Value         // read from database
        )
        { }
    }

    public class PromptComparer : ValueComparer<Prompt>
    {
        public PromptComparer()
        : base(
            (mv1, mv2) => mv1!.Value == mv2!.Value,     // equality
            mv => mv.Value.GetHashCode(),               // hashcode
            mv => Prompt.Create(mv.Value).Value         // snapshot (deep copy)
        )
        { }
    }
}