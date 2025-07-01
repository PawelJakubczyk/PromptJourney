using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Commands.DeleteParameterInVersion;

public class DeleteParameterInVersionCommandHandler : IRequestHandler<DeleteParameterInVersionCommand, Result<ParameterDetails>>
{
    private readonly IVersionRepository _versionRepository;

    public DeleteParameterInVersionCommandHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<ParameterDetails>> Handle(DeleteParameterInVersionCommand request, CancellationToken cancellationToken)
    {
        await Validate.Version.ShouldExists(request.Version, _versionRepository);
        await Validate.Parameter.ShouldExists(request.Version, request.PropertyName, _versionRepository);

        try
        {
            var result = await _versionRepository.DeleteParameterInVersionAsync(request.Version, request.PropertyName);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<ParameterDetails>($"Error deleting parameter: {ex.Message}");
        }
    }
}