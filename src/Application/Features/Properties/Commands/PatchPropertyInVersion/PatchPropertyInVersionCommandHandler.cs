using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Properties.Commands.PatchPropertyInVersion;

public class PatchPropertyInVersionCommandHandler : IRequestHandler<PatchPropertyInVersionCommand, Result<PropertyDetails>>
{
    private readonly IVersionRepository _versionRepository;
    private readonly IPropertiesRopository _propertiesRepository;

    public PatchPropertyInVersionCommandHandler(IVersionRepository versionRepository, IPropertiesRopository propertiesRepository)
    {
        _versionRepository = versionRepository;
        _propertiesRepository = propertiesRepository;
    }

    public async Task<Result<PropertyDetails>> Handle(PatchPropertyInVersionCommand request, CancellationToken cancellationToken)
    {
        await Validate.Version.ShouldExists(request.Version, _versionRepository);
        await Validate.Property.ShouldExists(request.Version, request.PropertyName, _propertiesRepository);

        try
        {
            var result = await _propertiesRepository.PatchParameterInVersionAsync
            (
                request.Version,
                request.PropertyName,
                request.PropertyToUpdate,
                request.NewValue
            );

            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<PropertyDetails>($"Error patching parameter: {ex.Message}");
        }
    }
}