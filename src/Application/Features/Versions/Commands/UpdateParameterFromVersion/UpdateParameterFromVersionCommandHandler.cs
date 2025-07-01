using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Commands.UpdateParameterFromVersion;

public class UpdateParameterFromVersionCommandHandler : IRequestHandler<UpdateParameterFromVersionCommand, Result<ParameterDetails>>
{
    private readonly IVersionRepository _versionRepository;

    public UpdateParameterFromVersionCommandHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<ParameterDetails>> Handle(UpdateParameterFromVersionCommand request, CancellationToken cancellationToken)
    {
        await Validate.Version.ShouldExists(request.Version, _versionRepository);
        await Validate.Parameter.ShouldExists(request.Version, request.PropertyName, _versionRepository);

        try
        {
            var result = await _versionRepository.UpdateParameterFromVersionAsync
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
            return Result.Fail<ParameterDetails>($"Error updating parameter: {ex.Message}");
        }
    }
}