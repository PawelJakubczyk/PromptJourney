//using Application.Abstractions;
//using FluentResults;
//using System.Text.RegularExpressions;

//namespace Utilities.Validation;

//public sealed class Validate
//{
//    public static class Parameter
//    {
//        public static async Task<Result> ShouldExists(string version, string propertyName, IVersionRepository repository)
//        {
//            if (string.IsNullOrWhiteSpace(propertyName))
//                return Result.Fail("Property name cannot be null or empty");

//            var result = await repository.CheckParameterExistsInVersionAsync(version, propertyName);
            
//            if (result.IsFailed)
//                return Result.Fail(result.Errors);
                
//            if (!result.Value)
//                return Result.Fail($"Parameter '{propertyName}' does not exist for version '{version}'");
                
//            return Result.Ok();
//        }
        
//        public static async Task<Result> ShouldNotExists(string version, string propertyName, IVersionRepository repository)
//        {
//            if (string.IsNullOrWhiteSpace(propertyName))
//                return Result.Fail("Property name cannot be null or empty");

//            var result = await repository.CheckParameterExistsInVersionAsync(version, propertyName);
            
//            if (result.IsFailed)
//                return Result.Fail(result.Errors);
                
//            if (result.Value)
//                return Result.Fail($"Parameter '{propertyName}' already exists for version '{version}'");
                
//            return Result.Ok();
//        }
        
//        public static Result ValidateName(string propertyName)
//        {
//            if (string.IsNullOrWhiteSpace(propertyName))
//                return Result.Fail("Property name cannot be null or empty");
                
//            if (propertyName.Length > 25)
//                return Result.Fail($"Property name too long: {propertyName.Length} (max: 25)");
                
//            // Property names should be camelCase
//            if (!Regex.IsMatch(propertyName, @"^[a-z][a-zA-Z0-9]*$"))
//                return Result.Fail($"Property name must be in camelCase format: {propertyName}");
                
//            return Result.Ok();
//        }
        
//        public static Result ValidateParameters(string[] parameters)
//        {
//            if (parameters == null || parameters.Length == 0)
//                return Result.Fail("Parameters array cannot be null or empty");
                
//            if (parameters.Length > 10)
//                return Result.Fail($"Too many parameters: {parameters.Length} (max: 10)");
                
//            foreach (var param in parameters)
//            {
//                if (string.IsNullOrWhiteSpace(param))
//                    return Result.Fail("Parameter values cannot be null or empty");
                
//                if (param.Length > 100)
//                    return Result.Fail($"Parameter value too long: {param.Length} (max: 100)");
//            }
            
//            return Result.Ok();
//        }
        
//        public static Result ValidatePropertyToUpdate(string propertyToUpdate)
//        {
//            if (string.IsNullOrWhiteSpace(propertyToUpdate))
//                return Result.Fail("Property to update cannot be null or empty");
                
//            var allowedProperties = new[] { "DefaultValue", "MinValue", "MaxValue", "Description", "Parameters" };
            
//            if (!allowedProperties.Contains(propertyToUpdate, StringComparer.OrdinalIgnoreCase))
//                return Result.Fail(
//                    $"Property '{propertyToUpdate}' is not supported for updates. " +
//                    $"Supported properties: {string.Join(", ", allowedProperties)}");
                    
//            return Result.Ok();
//        }
//    }

//    public static class Version
//    {
//        public static async Task<Result> ShouldExists(string version, IVersionRepository repository)
//        {
//            if (string.IsNullOrWhiteSpace(version))
//                return Result.Fail("Version cannot be null or empty");
                
//            var result = await repository.CheckVersionExistsInVersionMasterAsync(version);
            
//            if (result.IsFailed)
//                return Result.Fail(result.Errors);
                
//            if (!result.Value)
//                return Result.Fail($"Version '{version}' does not exist in the system");
                
//            return Result.Ok();
//        }
        
//        public static Result ValidateFormat(string version)
//        {
//            if (string.IsNullOrWhiteSpace(version))
//                return Result.Fail("Version cannot be null or empty");
                
//            if (version.Length > 10)
//                return Result.Fail($"Version is too long: {version.Length} (max: 10)");
                
//            // Check version format (numeric, decimal, or "niji N" format)
//            var isValidNumeric = Regex.IsMatch(version, @"^[1-9](\.[0-9])?$");
//            var isValidNiji = Regex.IsMatch(version, @"^niji [4-6]$");
            
//            if (!isValidNumeric && !isValidNiji)
//                return Result.Fail($"Invalid version format: {version}. Expected numeric (e.g., '5', '5.1') or niji format (e.g., 'niji 5')");
                
//            return Result.Ok();
//        }
        
//        public static string Normalize(string version)
//        {
//            // Convert "niji4" to "niji 4" format if needed
//            if (version.StartsWith("niji", StringComparison.OrdinalIgnoreCase) && 
//                !version.Contains(" ") && 
//                version.Length > 4)
//            {
//                return $"niji {version[4..]}";
//            }
            
//            return version;
//        }
        
//        public static readonly string[] SupportedVersions = new[]
//        {
//            "1", "2", "3", "4", "5", "5.1", "5.2", "6", "6.1", "7", "niji 4", "niji 5", "niji 6"
//        };
//    }
//}