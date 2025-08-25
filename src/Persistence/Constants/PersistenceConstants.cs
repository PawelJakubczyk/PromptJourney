namespace Persistence.Constants;

public static partial class PersistenceConstants
{
    internal static class ColumnType
    {
        internal const string uuid = nameof(uuid);
        internal const string smallint = nameof(smallint);
        internal const string boolean = nameof(boolean);
        internal const string text = nameof(text);
        internal const string textArray = $"{text}[]";
        internal const string uuidArray = $"{uuid}[]";
        internal const string jsonb = nameof(jsonb);
        internal static string bytea = nameof(bytea);

        internal static string TimestampWithTimeZone() => "timestamp with time zone";
        internal static string Char(int length) => $"char({length})";
        internal static string VarChar(int length) => $"varchar({length})";

        ////internal const string Bit = nameof(Bit);
        //internal static string TimestampWithTimeZone(int length) => $"{nameof(TimestampWithTimeZone)}({length})";
        //internal static string NChar(int length) => $"{nameof(NChar)}({length})";
        //internal static string VarChar(int length) => $"{nameof(VarChar)}({length})";
        ////internal static string VarChar(int length) => $"{nameof(VarChar)}({length})";
        //internal static string Char(int length) => $"{nameof(Char)}({length})";
        //internal static string Binary(int length) => $"{nameof(Binary)}({length})";
    }
}

//public static partial class Constants
//{
//    internal static class ColumnType
//    {
//        //internal const string Uuid = "uuid";
//        //internal const string SmallInt = "smallint";
//        //internal const string Boolean = "boolean";
//        //internal const string Text = "text";
//        //internal const string TextArray = "text[]";
//        //internal const string UuidArray = "uuid[]";
//        //internal const string jsonb = "jsonb";

        
        
        
//        ; // odpowiednik Binary

//        // Możesz też dodać timestamp bez strefy:
//        internal static string TimestampWithoutTimeZone() => "timestamp without time zone";
//    }
//}
