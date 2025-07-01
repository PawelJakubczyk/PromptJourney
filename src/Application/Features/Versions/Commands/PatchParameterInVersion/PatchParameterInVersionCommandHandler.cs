using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Commands.PatchParameterInVersion;

public class PatchParameterInVersionCommandHandler : IRequestHandler<PatchParameterInVersionCommand, Result<ParameterDetails>>
{
    private readonly IVersionRepository _versionRepository;

    public PatchParameterInVersionCommandHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<ParameterDetails>> Handle(PatchParameterInVersionCommand request, CancellationToken cancellationToken)
    {
        await Validate.Version.ShouldExists(request.Version, _versionRepository);
        await Validate.Parameter.ShouldExists(request.Version, request.PropertyName, _versionRepository);

        try
        {
            var result = await _versionRepository.PatchParameterInVersionAsync
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
            return Result.Fail<ParameterDetails>($"Error patching parameter: {ex.Message}");
        }
    }
}