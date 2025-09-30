using FluentResults;
using Utilities.Constants;

namespace Utilities.Errors;

public class Error<TLayer> : Error
    where TLayer : ILayer
{
    public LayersEnum Layer { get; }
    public int ErrorCode { get; }

    public Error(string message, int errorCode) : base($"[{ResolveLayer()}] {message}")
    {
        Layer = ResolveLayer();
        ErrorCode = errorCode;
    }

    private static LayersEnum ResolveLayer()
    {
        return typeof(TLayer) switch
        {
            var t when t == typeof(DomainLayer) => LayersEnum.Domain,
            var t when t == typeof(ApplicationLayer) => LayersEnum.Application,
            var t when t == typeof(PersistenceLayer) => LayersEnum.Persistence,
            var t when t == typeof(InfrastructureLayer) => LayersEnum.Infrastructure,
            var t when t == typeof(PresentationLayer) => LayersEnum.Presentation,
            var t when t == typeof(UtilitiesLayer) => LayersEnum.Utilities,
            _ => LayersEnum.Unknown
        };
    }
}