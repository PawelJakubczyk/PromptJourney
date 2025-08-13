using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.VersionsMaster.Queries.CheckVersionExists;

public sealed class CheckVersionExistsQueryHandler : IRequestHandler<CheckVersionExistsQuery, Result<bool>>
{
    private readonly IVersionRepository _versionRepository;

    public CheckVersionExistsQueryHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<bool>> Handle(CheckVersionExistsQuery request, CancellationToken cancellationToken)
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