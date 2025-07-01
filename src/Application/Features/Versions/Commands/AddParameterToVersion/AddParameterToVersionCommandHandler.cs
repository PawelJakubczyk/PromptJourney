using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Commands.AddParameterToVersion;

public class AddParameterToVersionCommandHandler : IRequestHandler<AddParameterToVersionCommand, Result<ParameterDetails>>
{
    private readonly IVersionRepository _versionRepository;

    public AddParameterToVersionCommandHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<ParameterDetails>> Handle(AddParameterToVersionCommand request, CancellationToken cancellationToken)
    {
        await Validate.Version.ShouldExists(request.Version, _versionRepository);
        await Validate.Parameter.ShouldNotExists(request.Version, request.PropertyName, _versionRepository);

        try
        {
            var update = await _versionRepository.AddParameterToVersionAsync
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
            return Result.Fail<ParameterDetails>($"Error adding parameter: {ex.Message}");
        }
    }
}
