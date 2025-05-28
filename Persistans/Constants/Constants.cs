namespace Persistans.Constants;

public static partial class Constants
{
    internal static class ColumnType
    {
        internal const string UniqueIdentifier = nameof(UniqueIdentifier);
        internal const string TinyInt = nameof(TinyInt);
        internal const string Bit = nameof(Bit);
        internal const string Text = "text";
        internal const string TextArray = "text[]";
        internal const string UuidArray = "uuid[]";
        internal const string Jsonb = "jsonb";
        internal static string DateTimeOffset(int lenght) => $"{nameof(DateTimeOffset)}({lenght})";
        internal static string NChar(int lenght) => $"{nameof(NChar)}({lenght})";
        internal static string VarChar(int lenght) => $"{nameof(VarChar)}({lenght})";
        internal static string NVarChar(int lenght) => $"{nameof(NVarChar)}({lenght})";
        internal static string Char(int lenght) => $"{nameof(Char)}({lenght})";
        internal static string Binary(int lenght) => $"{nameof(Binary)}({lenght})";
    }
}