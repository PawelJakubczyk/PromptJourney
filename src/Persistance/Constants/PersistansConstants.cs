namespace Persistance.Constants;

public static partial class PersistansConstants
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
        //internal static string TimestampWithTimeZone(int lenght) => $"{nameof(TimestampWithTimeZone)}({lenght})";
        //internal static string NChar(int lenght) => $"{nameof(NChar)}({lenght})";
        //internal static string VarChar(int lenght) => $"{nameof(VarChar)}({lenght})";
        ////internal static string VarChar(int lenght) => $"{nameof(VarChar)}({lenght})";
        //internal static string Char(int lenght) => $"{nameof(Char)}({lenght})";
        //internal static string Binary(int lenght) => $"{nameof(Binary)}({lenght})";
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
