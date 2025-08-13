using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Properties.Commands.AddPropertyInVersion;

public class AddPropertyInVersionCommandHandler : IRequestHandler<AddPropertyInVersionCommand, Result<PropertyDetails>>
{
    private readonly IVersionRepository _versionRepository;
    private readonly IPropertiesRopository _propertiesRepository;

    public AddPropertyInVersionCommandHandler(IVersionRepository versionRepository, IPropertiesRopository propertiesRepository)
    {
        _versionRepository = versionRepository;
        _propertiesRepository = propertiesRepository;
    }

    public async Task<Result<PropertyDetails>> Handle(AddPropertyInVersionCommand request, CancellationToken cancellationToken)
    {
        await Validate.Version.ShouldExists(request.Version, _versionRepository);
        await Validate.Property.ShouldNotExists(request.Version, request.PropertyName, _propertiesRepository);

        try
        {
            var update = await _propertiesRepository.AddParameterToVersionAsync
            (
                request.Version,
                request.PropertyName,
                request.Parameters,
                request.DefaultValue,
                request.MinValue,
                request.MaxValue,
                request.Description
            );

            return Result.Ok(update.Value);
        }
        catch (Exception ex)
        {
            return Result.Fail<PropertyDetails>($"Error adding parameter: {ex.Message}");
        }
    }
}
