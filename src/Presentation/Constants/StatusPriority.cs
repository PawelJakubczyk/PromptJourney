using Microsoft.AspNetCore.Http;

namespace Presentation.Constants;

public static class StatusPriority
{
    public static Dictionary<int, int> StatusPriorityDict = new()
    {
        // 1xx Information (lowest priority)
        [StatusCodes.Status100Continue] = 9,
        [StatusCodes.Status101SwitchingProtocols] = 9,
        [StatusCodes.Status102Processing] = 9,

        // 2xx Success (very low priority)
        [StatusCodes.Status200OK] = 8,
        [StatusCodes.Status201Created] = 8,
        [StatusCodes.Status202Accepted] = 8,
        [StatusCodes.Status203NonAuthoritative] = 8,
        [StatusCodes.Status204NoContent] = 8,
        [StatusCodes.Status205ResetContent] = 8,
        [StatusCodes.Status206PartialContent] = 8,
        [StatusCodes.Status207MultiStatus] = 8,
        [StatusCodes.Status208AlreadyReported] = 8,

        // 3xx redirection (low priority; usually not client/server errors)
        [StatusCodes.Status300MultipleChoices] = 7,
        [StatusCodes.Status301MovedPermanently] = 7,
        [StatusCodes.Status302Found] = 7,
        [StatusCodes.Status303SeeOther] = 7,
        [StatusCodes.Status304NotModified] = 7,
        [StatusCodes.Status305UseProxy] = 7,
        [StatusCodes.Status306SwitchProxy] = 7,
        [StatusCodes.Status307TemporaryRedirect] = 7,
        [StatusCodes.Status308PermanentRedirect] = 7,

        // 4xx client errors (higher priority than 2xx/3xx; distinctions inside the class)
        // The most important for domain logic: conflict, Throttling, Auth
        [StatusCodes.Status400BadRequest] = 5,
        [StatusCodes.Status401Unauthorized] = 4,
        [StatusCodes.Status402PaymentRequired] = 6,
        [StatusCodes.Status403Forbidden] = 4,
        [StatusCodes.Status404NotFound] = 3,
        [StatusCodes.Status405MethodNotAllowed] = 5,
        [StatusCodes.Status406NotAcceptable] = 5,
        [StatusCodes.Status407ProxyAuthenticationRequired] = 5,
        [StatusCodes.Status408RequestTimeout] = 4,
        [StatusCodes.Status409Conflict] = 2,         // High priority in 4xx (state conflict)
        [StatusCodes.Status410Gone] = 4,
        [StatusCodes.Status411LengthRequired] = 5,
        [StatusCodes.Status412PreconditionFailed] = 5,
        [StatusCodes.Status413PayloadTooLarge] = 5,
        [StatusCodes.Status414UriTooLong] = 5,
        [StatusCodes.Status415UnsupportedMediaType] = 5,
        [StatusCodes.Status416RangeNotSatisfiable] = 5,
        [StatusCodes.Status417ExpectationFailed] = 5,
        [StatusCodes.Status418ImATeapot] = 6,
        [StatusCodes.Status421MisdirectedRequest] = 5,
        [StatusCodes.Status422UnprocessableEntity] = 5,
        [StatusCodes.Status423Locked] = 5,
        [StatusCodes.Status424FailedDependency] = 5,
        [StatusCodes.Status426UpgradeRequired] = 5,
        [StatusCodes.Status428PreconditionRequired] = 5,
        [StatusCodes.Status429TooManyRequests] = 2,   // High Priority (Throttling)
        [StatusCodes.Status431RequestHeaderFieldsTooLarge] = 5,
        [StatusCodes.Status451UnavailableForLegalReasons] = 5,

        // 5xx Server Errors (highest priority)
        [StatusCodes.Status500InternalServerError] = 1,
        [StatusCodes.Status501NotImplemented] = 1,
        [StatusCodes.Status502BadGateway] = 1,
        [StatusCodes.Status503ServiceUnavailable] = 1,
        [StatusCodes.Status504GatewayTimeout] = 1,
        [StatusCodes.Status505HttpVersionNotsupported] = 1,
        [StatusCodes.Status506VariantAlsoNegotiates] = 1,
        [StatusCodes.Status507InsufficientStorage] = 1,
        [StatusCodes.Status508LoopDetected] = 1,
        [StatusCodes.Status511NetworkAuthenticationRequired] = 1
    };
}