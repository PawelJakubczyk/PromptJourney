using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Properties.Commands.UpdatePropertyInVersion;

public class UpdatePropertyInVersionCommandHandler : IRequestHandler<UpdatePropertyInVersionCommand, Result<PropertyDetails>>
{
    private readonly IVersionRepository _versionRepository;
    private readonly IPropertiesRopository _propertiesRepository;

    public UpdatePropertyInVersionCommandHandler(IVersionRepository versionRepository, IPropertiesRopository propertiesRepository)
    {
        _versionRepository = versionRepository;
        _propertiesRepository = propertiesRepository;
    }

    public async Task<Result<PropertyDetails>> Handle(UpdatePropertyInVersionCommand request, CancellationToken cancellationToken)
    {
        await Validate.Version.ShouldExists(request.Version, _versionRepository);
        await Validate.Property.ShouldExists(request.Version, request.PropertyName, _propertiesRepository);

        try
        {
            var result = await _propertiesRepository.UpdateParameterFromVersionAsync
            (
                request.Version,
                request.PropertyName,
                request.Parameters,
                request.DefaultValue,
                request.MinValue,
                request.MaxValue,
                request.Description
            );

            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<PropertyDetails>($"Error updating parameter: {ex.Message}");
        }
    }
}