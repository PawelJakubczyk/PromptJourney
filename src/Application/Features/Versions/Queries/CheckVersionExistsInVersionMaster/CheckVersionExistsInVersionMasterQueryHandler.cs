using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Queries.CheckVersionExistsInVersionMaster;

public sealed class CheckVersionExistsInVersionMasterQueryHandler : IRequestHandler<CheckVersionExistsInVersionMasterQuery, Result<bool>>
{
    private readonly IVersionRepository _versionRepository;

    public CheckVersionExistsInVersionMasterQueryHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<bool>> Handle(CheckVersionExistsInVersionMasterQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _versionRepository.CheckVersionExistsInVersionMasterAsync(request.Version);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Error checking version existence: {ex.Message}");
        }
    }
}