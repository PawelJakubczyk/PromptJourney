namespace Persistence.Constants;

public static partial class PersistenceConstants
{
    internal static class ColumnType
    {
        internal const string Uuid = nameof(Uuid);
        internal const string SmallInt = nameof(SmallInt);
        internal const string Boolean = nameof(Boolean);
        internal const string Text = nameof(Text);
        internal const string TextArray = $"{Text}[]";
        internal const string UuidArray = $"{Uuid}[]";
        internal const string Jsonb = nameof(Jsonb);
        internal static string Bytea = nameof(Bytea);

        internal static string TimestampWithTimeZone() => "timestamp with time zone";
        internal static string Char(int length) => $"char({length})";
        internal static string VarChar(int length) => $"varchar({length})";
    }
}