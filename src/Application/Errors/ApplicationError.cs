using FluentResults;

namespace Application.Errors;

public static class ApplicationErrorMessages
{
    public class ApplicationError : Error
    {
        public ApplicationError(string message) : base(message)
        {
        }

        public ApplicationError WithDetail(string detail)
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

