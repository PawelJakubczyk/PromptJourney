//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json.Serialization;
//using System.Threading.Tasks;
//using System.Text.Json.Serialization;

//namespace Domain.ValueObjects;

//public class ParameterDefinitionRange
//{
//    [JsonPropertyName("min")]
//    public int Min { get; set; }

//    [JsonPropertyName("max")]
//    public int Max { get; set; }

//    [JsonPropertyName("default")]
//    public int Default { get; set; }

//    [JsonPropertyName("description")]
//    public string? Description { get; set; }

//    [JsonPropertyName("abbreviation")]
//    public string? Abbreviation { get; set; }
//}

//public class ParameterDefinitionFlag
//{
//    [JsonPropertyName("abbreviation")]
//    public string? Abbreviation { get; set; }

//    [JsonPropertyName("description")]
//    public string? Description { get; set; }
//}

//public class ParameterDefinitionModes
//{
//    // { "fast": "--fast", "relax": "--relax", "turbo": null }
//    [JsonPropertyName("modes")]
//    public Dictionary<string, string?> Modes { get; set; } = new();

//    [JsonPropertyName("default")]
//    public string Default { get; set; } = null!;

//    [JsonPropertyName("description")]
//    public string? Description { get; set; }
//}

//public class ParameterDefinitionTestTestp
//{
//    [JsonPropertyName("test")]
//    public string Test { get; set; } = null!;

//    [JsonPropertyName("testp")]
//    public string Testp { get; set; } = null!;

//    [JsonPropertyName("description")]
//    public string? Description { get; set; }
//}

//public class ParameterDefinitionOptions
//{
//    [JsonPropertyName("options")]
//    public List<string> Options { get; set; } = new();

//    [JsonPropertyName("description")]
//    public string? Description { get; set; }

//    [JsonPropertyName("abbreviation")]
//    public string? Abbreviation { get; set; }
//}