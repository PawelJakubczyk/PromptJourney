using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Properties.Commands.DeletePropertyInVersion;

public class DeletePropertyInVersionCommandHandler : IRequestHandler<DeletePropertyInVersionCommand, Result<PropertyDetails>>
{
    private readonly IVersionRepository _versionRepository;
    private readonly IPropertiesRopository _propertiesRepository;

    public DeletePropertyInVersionCommandHandler(IVersionRepository versionRepository, IPropertiesRopository propertiesRepository)
    {
        _versionRepository = versionRepository;
        _propertiesRepository = propertiesRepository;
    }

    public async Task<Result<PropertyDetails>> Handle(DeletePropertyInVersionCommand request, CancellationToken cancellationToken)
    {
        await Validate.Version.ShouldExists(request.Version, _versionRepository);
        await Validate.Property.ShouldExists(request.Version, request.PropertyName, _propertiesRepository);

        try
        {
            var result = await _propertiesRepository.DeleteParameterInVersionAsync(request.Version, request.PropertyName);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<PropertyDetails>($"Error deleting parameter: {ex.Message}");
        }
    }
}