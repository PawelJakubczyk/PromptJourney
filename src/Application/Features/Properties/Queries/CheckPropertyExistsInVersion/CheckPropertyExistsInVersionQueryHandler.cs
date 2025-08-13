using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Properties.Queries.CheckPropertyExistsInVersion;

public sealed class CheckPropertyExistsInVersionQueryHandler : IRequestHandler<CheckPropertyExistsInVersionQuery, Result<bool>>
{
    private readonly IPropertiesRopository _propertiesRepository;

    public CheckPropertyExistsInVersionQueryHandler(IPropertiesRopository propertiesRepository)
    {
        _propertiesRepository = propertiesRepository;
    }

    public async Task<Result<bool>> Handle(CheckPropertyExistsInVersionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _propertiesRepository.CheckParameterExistsInVersionAsync(request.Version, request.PropertyName);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Error checking parameter existence: {ex.Message}");
        }
    }
}