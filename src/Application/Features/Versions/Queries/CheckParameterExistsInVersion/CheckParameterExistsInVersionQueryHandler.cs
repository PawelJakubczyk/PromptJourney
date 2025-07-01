using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Queries.CheckParameterExistsInVersion;

public sealed class CheckParameterExistsInVersionQueryHandler : IRequestHandler<CheckParameterExistsInVersionQuery, Result<bool>>
{
    private readonly IVersionRepository _versionRepository;

    public CheckParameterExistsInVersionQueryHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<bool>> Handle(CheckParameterExistsInVersionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _versionRepository.CheckParameterExistsInVersionAsync(request.Version, request.PropertyName);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Error checking parameter existence: {ex.Message}");
        }
    }
}