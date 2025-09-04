using Domain.Errors;

namespace Application.Errors;

class ErrorTools
{
    public bool IfAtLastOneError<TType>
    (
        List<ApplicationError> applicationErrors,
        List<DomainError> domainErrors
    ) where TType : class
    {
        if (applicationErrors.Count != 0 || domainErrors.Count != 0)
        {
        }
        return false;
    }

}
