//using Domain.Entities.MidjourneyVersions;
//using MediatR;
//using Microsoft.EntityFrameworkCore;

//namespace Application.Features.Versions.Commands.AddParameterToVersion;

//public class AddParameterToVersionCommandHandler : IRequestHandler<AddParameterToVersionCommand, AddParameterToVersionResponse>
//{
//    private readonly MidjourneyDbContext _context;

//    public AddParameterToVersionCommandHandler(MidjourneyDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<AddParameterToVersionResponse> Handle(AddParameterToVersionCommand request, CancellationToken cancellationToken)
//    {
//        // Validate if version exists
//        var versionMaster = await _context.MidjourneyVersionsMaster
//            .FirstOrDefaultAsync(v => v.Version == request.Version, cancellationToken);

//        if (versionMaster == null)
//        {
//            return AddParameterToVersionResponse.FailureResult($"Version '{request.Version}' not found");
//        }

//        // Normalize version string for table access
//        string tableVersion = request.Version.Replace(".", "_").Replace(" ", "_").ToLower();

//        // Check if parameter already exists in this version
//        bool parameterExists = await CheckParameterExistsAsync(tableVersion, request.PropertyName, cancellationToken);
//        if (parameterExists)
//        {
//            return AddParameterToVersionResponse.FailureResult($"Parameter '{request.PropertyName}' already exists for version '{request.Version}'");
//        }

//        // Add parameter to the appropriate version table
//        try
//        {
//            var success = await AddParameterToVersionTableAsync(tableVersion, request, cancellationToken);
//            if (!success)
//            {
//                return AddParameterToVersionResponse.FailureResult("Failed to add parameter to version");
//            }

//            // Return success result
//            return AddParameterToVersionResponse.SuccessResult(new ParameterDetails
//            {
//                Version = request.Version,
//                PropertyName = request.PropertyName,
//                Parameters = request.Parameters,
//                DefaultValue = request.DefaultValue,
//                MinValue = request.MinValue,
//                MaxValue = request.MaxValue,
//                Description = request.Description
//            });
//        }
//        catch (Exception ex)
//        {
//            return AddParameterToVersionResponse.FailureResult($"Error adding parameter: {ex.Message}");
//        }
//    }

//    private async Task<bool> CheckParameterExistsAsync(string tableVersion, string propertyName, CancellationToken cancellationToken)
//    {
//        return tableVersion switch
//        {
//            "1" => await _context.MidjourneyVersion1.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "2" => await _context.MidjourneyVersion2.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "3" => await _context.MidjourneyVersion3.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "4" => await _context.MidjourneyVersion4.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "5" => await _context.MidjourneyVersion5.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "5_1" => await _context.MidjourneyVersion51.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "5_2" => await _context.MidjourneyVersion52.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "6" => await _context.MidjourneyVersion6.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "6_1" => await _context.MidjourneyVersion61.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "7" => await _context.MidjourneyVersion7.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "niji_4" => await _context.MidjourneyVersionNiji4.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "niji_5" => await _context.MidjourneyVersionNiji5.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            "niji_6" => await _context.MidjourneyVersionNiji6.AnyAsync(p => p.PropertyName == propertyName, cancellationToken),
//            _ => false
//        };
//    }

//    private async Task<bool> AddParameterToVersionTableAsync(string tableVersion, AddParameterToVersionCommand request, CancellationToken cancellationToken)
//    {
//        // Get the version master to link
//        var versionMaster = await _context.MidjourneyVersionsMaster
//            .FirstAsync(v => v.Version == request.Version, cancellationToken);

//        switch (tableVersion)
//        {
//            case "1":
//                _context.MidjourneyVersion1.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersion1>(request, versionMaster));
//                break;
//            case "2":
//                _context.MidjourneyVersion2.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersion2>(request, versionMaster));
//                break;
//            case "3":
//                _context.MidjourneyVersion3.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersion3>(request, versionMaster));
//                break;
//            case "4":
//                _context.MidjourneyVersion4.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersion4>(request, versionMaster));
//                break;
//            case "5":
//                _context.MidjourneyVersion5.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersion5>(request, versionMaster));
//                break;
//            case "5_1":
//                _context.MidjourneyVersion51.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersion51>(request, versionMaster));
//                break;
//            case "5_2":
//                _context.MidjourneyVersion52.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersion52>(request, versionMaster));
//                break;
//            case "6":
//                _context.MidjourneyVersion6.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersion6>(request, versionMaster));
//                break;
//            case "6_1":
//                _context.MidjourneyVersion61.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersion61>(request, versionMaster));
//                break;
//            case "7":
//                _context.MidjourneyVersion7.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersion7>(request, versionMaster));
//                break;
//            case "niji_4":
//                _context.MidjourneyVersionNiji4.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersionNiji4>(request, versionMaster));
//                break;
//            case "niji_5":
//                _context.MidjourneyVersionNiji5.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersionNiji5>(request, versionMaster));
//                break;
//            case "niji_6":
//                _context.MidjourneyVersionNiji6.Add(CreateParameter<MidjourneyAllVersions.MidjourneyVersionNiji6>(request, versionMaster));
//                break;
//            default:
//                return false;
//        }

//        await _context.SaveChangesAsync(cancellationToken);
//        return true;
//    }

//    private static T CreateParameter<T>(AddParameterToVersionCommand request, MidjourneyVersionsMaster versionMaster) where T : MidjourneyVersionsBase, new()
//    {
//        return new T
//        {
//            PropertyName = request.PropertyName,
//            Version = request.Version,
//            Parameters = request.Parameters,
//            DefaultValue = request.DefaultValue,
//            MinValue = request.MinValue,
//            MaxValue = request.MaxValue,
//            Description = request.Description,
//            VersionMaster = versionMaster
//        };
//    }
//}