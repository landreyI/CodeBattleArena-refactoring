
using System.Net;

namespace CodeBattleArena.Domain.Common
{
    public class DomainException : Exception
    {
        // По умолчанию 400
        public HttpStatusCode StatusCode { get; }

        // Код ошибки для фронтенда
        public string ErrorCode { get; }

        public DomainException(
            string message,
            string errorCode = "domain_error",
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
