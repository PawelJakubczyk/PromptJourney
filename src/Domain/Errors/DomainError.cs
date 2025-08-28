using FluentResults;

namespace Domain.Errors;

public static class DomainErrorMessages
{
    public class DomainError : Error
    {
        public DomainError(string message) : base(message)
        {
        }

        public DomainError WithDetail(string detail)
        {
            this.Metadata.Add("Detail", CutOffDetail(detail));
            return this;
        }

        private static string CutOffDetail(string detail, int maxLength = 40)
        {
            return detail.Length <= maxLength
                ? detail
                : $"{detail[..maxLength]}...";
        }
    }
}

