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

        internal static string TimestampWithTimeZone() => "timestamp with time zone";

        internal static string VarChar(int length) => $"varchar({length})";
    }
}